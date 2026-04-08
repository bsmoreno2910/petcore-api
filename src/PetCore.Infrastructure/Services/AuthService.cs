using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetCore.Domain.Entities;
using PetCore.Domain.Interfaces;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _db.Users
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Clinic)
            .FirstOrDefaultAsync(u => u.Email == email && u.Active);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return new AuthResult { Success = false, Error = "E-mail ou senha inválidos." };

        var clinics = user.ClinicUsers
            .Where(cu => cu.Active)
            .Select(cu => new { cu.ClinicId, cu.Clinic.Name, cu.Clinic.TradeName, Role = cu.Role.ToString() })
            .ToList();

        // Se só tem uma clínica, já gera token com clinicId
        Guid? activeClinicId = clinics.Count == 1 ? clinics[0].ClinicId : null;
        string? activeRole = clinics.Count == 1 ? clinics[0].Role : null;

        var accessToken = GenerateAccessToken(user, activeClinicId, activeRole);
        var refreshToken = GenerateRefreshToken();

        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new { user.Id, user.Name, user.Email, user.Phone, user.Crmv, user.AvatarUrl },
            Clinics = clinics.Select(c => new { c.ClinicId, c.Name, c.TradeName, c.Role }).Cast<object>().ToList()
        };
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        // Simplified refresh: in production, store refresh tokens in DB
        // For now, just re-issue tokens (the refresh token is validated by existence)
        return new AuthResult { Success = false, Error = "Refresh token inválido." };
    }

    public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            throw new InvalidOperationException("Senha atual incorreta.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<AuthResult> SelectClinicAsync(Guid userId, Guid clinicId)
    {
        var user = await _db.Users
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Clinic)
            .FirstOrDefaultAsync(u => u.Id == userId && u.Active);

        if (user == null)
            return new AuthResult { Success = false, Error = "Usuário não encontrado." };

        var clinicUser = user.ClinicUsers.FirstOrDefault(cu => cu.ClinicId == clinicId && cu.Active);
        if (clinicUser == null)
            return new AuthResult { Success = false, Error = "Usuário não possui acesso a esta clínica." };

        var accessToken = GenerateAccessToken(user, clinicId, clinicUser.Role.ToString());
        var refreshToken = GenerateRefreshToken();

        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateAccessToken(User user, Guid? clinicId, string? role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
        };

        if (clinicId.HasValue)
            claims.Add(new Claim("clinicId", clinicId.Value.ToString()));

        if (!string.IsNullOrEmpty(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var expMinutes = int.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"] ?? "15");

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
