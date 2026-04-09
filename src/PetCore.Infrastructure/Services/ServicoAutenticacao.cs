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

public class ServicoAutenticacao : IServicoAutenticacao
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public ServicoAutenticacao(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<ResultadoAutenticacao> LoginAsync(string email, string senha)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.ClinicaUsuarios)
                .ThenInclude(cu => cu.Clinica)
            .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            return new ResultadoAutenticacao { Sucesso = false, Erro = "E-mail ou senha inválidos." };

        var clinicas = usuario.ClinicaUsuarios
            .Where(cu => cu.Ativo)
            .Select(cu => new { cu.ClinicaId, cu.Clinica.Nome, cu.Clinica.NomeFantasia, Perfil = cu.Perfil.ToString() })
            .ToList();

        Guid? clinicaAtivaId = clinicas.Count == 1 ? clinicas[0].ClinicaId : null;
        string? perfilAtivo = clinicas.Count == 1 ? clinicas[0].Perfil : null;

        var tokenAcesso = GerarTokenAcesso(usuario, clinicaAtivaId, perfilAtivo);
        var tokenAtualizacao = await CriarTokenAtualizacaoAsync(usuario.Id);

        return new ResultadoAutenticacao
        {
            Sucesso = true,
            TokenAcesso = tokenAcesso,
            TokenAtualizacao = tokenAtualizacao,
            Usuario = new { usuario.Id, usuario.Nome, usuario.Email, usuario.Telefone, usuario.Crmv, usuario.AvatarUrl },
            Clinicas = clinicas.Select(c => new { c.ClinicaId, c.Nome, c.NomeFantasia, c.Perfil }).Cast<object>().ToList()
        };
    }

    public async Task<ResultadoAutenticacao> RefreshTokenAsync(string tokenAtualizacao)
    {
        var token = await _db.Set<TokenAtualizacao>()
            .Include(t => t.Usuario).ThenInclude(u => u.ClinicaUsuarios).ThenInclude(cu => cu.Clinica)
            .FirstOrDefaultAsync(t => t.Token == tokenAtualizacao);

        if (token == null || !token.Ativo)
            return new ResultadoAutenticacao { Sucesso = false, Erro = "Token de atualização inválido ou expirado." };

        // Revogar token usado e criar novo (rotação)
        token.RevogadoEm = DateTime.UtcNow;
        var novoRefresh = await CriarTokenAtualizacaoAsync(token.UsuarioId);
        token.SubstituidoPor = novoRefresh;

        // Gerar novo access token
        var usuario = token.Usuario;
        var clinicaAtiva = usuario.ClinicaUsuarios.FirstOrDefault(cu => cu.Ativo);
        var novoAccess = GerarTokenAcesso(usuario, clinicaAtiva?.ClinicaId, clinicaAtiva?.Perfil.ToString());

        await _db.SaveChangesAsync();

        return new ResultadoAutenticacao { Sucesso = true, TokenAcesso = novoAccess, TokenAtualizacao = novoRefresh };
    }

    public async Task AlterarSenhaAsync(Guid usuarioId, string senhaAtual, string novaSenha)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId)
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(senhaAtual, usuario.SenhaHash))
            throw new InvalidOperationException("Senha atual incorreta.");

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<ResultadoAutenticacao> SelecionarClinicaAsync(Guid usuarioId, Guid clinicaId)
    {
        var usuario = await _db.Usuarios
            .Include(u => u.ClinicaUsuarios)
                .ThenInclude(cu => cu.Clinica)
            .FirstOrDefaultAsync(u => u.Id == usuarioId && u.Ativo);

        if (usuario == null)
            return new ResultadoAutenticacao { Sucesso = false, Erro = "Usuário não encontrado." };

        var clinicaUsuario = usuario.ClinicaUsuarios.FirstOrDefault(cu => cu.ClinicaId == clinicaId && cu.Ativo);
        if (clinicaUsuario == null)
            return new ResultadoAutenticacao { Sucesso = false, Erro = "Usuário não possui acesso a esta clínica." };

        var tokenAcesso = GerarTokenAcesso(usuario, clinicaId, clinicaUsuario.Perfil.ToString());
        var tokenAtualizacao = await CriarTokenAtualizacaoAsync(usuario.Id);

        return new ResultadoAutenticacao { Sucesso = true, TokenAcesso = tokenAcesso, TokenAtualizacao = tokenAtualizacao };
    }

    private string GerarTokenAcesso(Usuario usuario, Guid? clinicaId, string? perfil)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
        };

        if (clinicaId.HasValue)
            claims.Add(new Claim("clinicaId", clinicaId.Value.ToString()));

        if (!string.IsNullOrEmpty(perfil))
            claims.Add(new Claim(ClaimTypes.Role, perfil));

        var minutos = int.Parse(_config["JwtSettings:AccessTokenExpirationMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutos),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> CriarTokenAtualizacaoAsync(Guid usuarioId)
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        var tokenStr = Convert.ToBase64String(bytes);

        var diasExpiracao = int.Parse(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "7");

        var token = new TokenAtualizacao
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Token = tokenStr,
            ExpiraEm = DateTime.UtcNow.AddDays(diasExpiracao),
            CriadoEm = DateTime.UtcNow
        };

        _db.Set<TokenAtualizacao>().Add(token);
        await _db.SaveChangesAsync();

        return tokenStr;
    }
}
