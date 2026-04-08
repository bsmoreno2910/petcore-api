using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class HospitalizationService
{
    private readonly AppDbContext _db;

    public HospitalizationService(AppDbContext db)
    {
        _db = db;
    }

    private IQueryable<Hospitalization> BaseQuery(Guid clinicId) =>
        _db.Hospitalizations
            .Include(h => h.Patient).ThenInclude(p => p.Tutor)
            .Include(h => h.Patient).ThenInclude(p => p.Species)
            .Include(h => h.Veterinarian)
            .Include(h => h.Evolutions)
            .Where(h => h.ClinicId == clinicId);

    public async Task<(List<Hospitalization> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize,
        HospitalizationStatus? status, Guid? patientId)
    {
        var query = BaseQuery(clinicId);

        if (status.HasValue)
            query = query.Where(h => h.Status == status.Value);

        if (patientId.HasValue)
            query = query.Where(h => h.PatientId == patientId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(h => h.AdmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Hospitalization?> GetByIdAsync(Guid id, Guid clinicId)
    {
        return await BaseQuery(clinicId)
            .Include(h => h.Evolutions).ThenInclude(e => e.Veterinarian)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hospitalization> CreateAsync(
        Guid clinicId, Guid veterinarianId, Guid patientId,
        string? reason, string? cage, string? diet, string? notes, DateTime? admittedAt)
    {
        var hospitalization = new Hospitalization
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            PatientId = patientId,
            VeterinarianId = veterinarianId,
            Status = HospitalizationStatus.Active,
            Reason = reason,
            Cage = cage,
            Diet = diet,
            Notes = notes,
            AdmittedAt = admittedAt ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Hospitalizations.Add(hospitalization);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(hospitalization.Id, clinicId))!;
    }

    public async Task<Hospitalization?> UpdateAsync(Guid id, Guid clinicId, Action<Hospitalization> updateAction)
    {
        var hosp = await _db.Hospitalizations.FirstOrDefaultAsync(h => h.Id == id && h.ClinicId == clinicId);
        if (hosp == null) return null;

        updateAction(hosp);
        hosp.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Hospitalization?> DischargeAsync(Guid id, Guid clinicId, string? dischargeNotes)
    {
        var hosp = await _db.Hospitalizations.FirstOrDefaultAsync(h => h.Id == id && h.ClinicId == clinicId);
        if (hosp == null) return null;

        if (hosp.Status != HospitalizationStatus.Active)
            throw new InvalidOperationException("Apenas internações ativas podem receber alta.");

        hosp.Status = HospitalizationStatus.Discharged;
        hosp.DischargedAt = DateTime.UtcNow;
        hosp.DischargeNotes = dischargeNotes;
        hosp.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(id, clinicId);
    }

    public async Task<HospitalizationEvolution> AddEvolutionAsync(
        Guid hospitalizationId, Guid clinicId, Guid veterinarianId, HospitalizationEvolution evolution)
    {
        var hosp = await _db.Hospitalizations.FirstOrDefaultAsync(h => h.Id == hospitalizationId && h.ClinicId == clinicId)
            ?? throw new InvalidOperationException("Internação não encontrada.");

        if (hosp.Status != HospitalizationStatus.Active)
            throw new InvalidOperationException("Apenas internações ativas podem receber evolução.");

        evolution.Id = Guid.NewGuid();
        evolution.HospitalizationId = hospitalizationId;
        evolution.VeterinarianId = veterinarianId;
        evolution.CreatedAt = DateTime.UtcNow;

        _db.HospitalizationEvolutions.Add(evolution);
        await _db.SaveChangesAsync();

        return await _db.HospitalizationEvolutions
            .Include(e => e.Veterinarian)
            .FirstAsync(e => e.Id == evolution.Id);
    }

    public async Task<List<HospitalizationEvolution>> GetEvolutionsAsync(Guid hospitalizationId, Guid clinicId)
    {
        var hosp = await _db.Hospitalizations.FirstOrDefaultAsync(h => h.Id == hospitalizationId && h.ClinicId == clinicId);
        if (hosp == null) return [];

        return await _db.HospitalizationEvolutions
            .Include(e => e.Veterinarian)
            .Where(e => e.HospitalizationId == hospitalizationId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}
