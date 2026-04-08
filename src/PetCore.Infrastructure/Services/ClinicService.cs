using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ClinicService
{
    private readonly AppDbContext _db;

    public ClinicService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Clinic>> GetAllAsync()
    {
        return await _db.Clinics.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Clinic?> GetByIdAsync(Guid id)
    {
        return await _db.Clinics.FindAsync(id);
    }

    public async Task<Clinic> CreateAsync(Clinic clinic)
    {
        clinic.Id = Guid.NewGuid();
        clinic.CreatedAt = DateTime.UtcNow;
        clinic.UpdatedAt = DateTime.UtcNow;

        _db.Clinics.Add(clinic);
        await _db.SaveChangesAsync();
        return clinic;
    }

    public async Task<Clinic?> UpdateAsync(Guid id, Action<Clinic> updateAction)
    {
        var clinic = await _db.Clinics.FindAsync(id);
        if (clinic == null) return null;

        updateAction(clinic);
        clinic.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return clinic;
    }

    public async Task<bool> ToggleActiveAsync(Guid id)
    {
        var clinic = await _db.Clinics.FindAsync(id);
        if (clinic == null) return false;

        clinic.Active = !clinic.Active;
        clinic.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ClinicUser>> GetClinicUsersAsync(Guid clinicId)
    {
        return await _db.ClinicUsers
            .Include(cu => cu.User)
            .Where(cu => cu.ClinicId == clinicId)
            .ToListAsync();
    }

    public async Task<ClinicUser> AddUserToClinicAsync(Guid clinicId, Guid userId, UserRole role)
    {
        var exists = await _db.ClinicUsers
            .AnyAsync(cu => cu.ClinicId == clinicId && cu.UserId == userId);
        if (exists)
            throw new InvalidOperationException("Usuário já vinculado a esta clínica.");

        var clinicUser = new ClinicUser
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            UserId = userId,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _db.ClinicUsers.Add(clinicUser);
        await _db.SaveChangesAsync();

        return await _db.ClinicUsers
            .Include(cu => cu.User)
            .FirstAsync(cu => cu.Id == clinicUser.Id);
    }

    public async Task<bool> RemoveUserFromClinicAsync(Guid clinicId, Guid userId)
    {
        var clinicUser = await _db.ClinicUsers
            .FirstOrDefaultAsync(cu => cu.ClinicId == clinicId && cu.UserId == userId);

        if (clinicUser == null) return false;

        _db.ClinicUsers.Remove(clinicUser);
        await _db.SaveChangesAsync();
        return true;
    }
}
