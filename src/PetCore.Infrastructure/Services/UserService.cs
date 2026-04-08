using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Clinic)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Clinic)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(string name, string email, string password, string? phone, string? crmv)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == email);
        if (emailExists)
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Phone = phone,
            Crmv = crmv,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(Guid id, Action<User> updateAction)
    {
        var user = await _db.Users
            .Include(u => u.ClinicUsers)
                .ThenInclude(cu => cu.Clinic)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        updateAction(user);
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return false;

        user.Active = !user.Active;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
