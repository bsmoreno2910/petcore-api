using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class PatientService
{
    private readonly AppDbContext _db;

    public PatientService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Patient> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize, string? search, Guid? speciesId, Guid? tutorId)
    {
        var query = _db.Patients
            .Include(p => p.Tutor)
            .Include(p => p.Species)
            .Include(p => p.Breed)
            .Where(p => p.ClinicId == clinicId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Tutor.Name.Contains(search));

        if (speciesId.HasValue)
            query = query.Where(p => p.SpeciesId == speciesId.Value);

        if (tutorId.HasValue)
            query = query.Where(p => p.TutorId == tutorId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Patient?> GetByIdAsync(Guid id, Guid clinicId)
    {
        return await _db.Patients
            .Include(p => p.Tutor)
            .Include(p => p.Species)
            .Include(p => p.Breed)
            .FirstOrDefaultAsync(p => p.Id == id && p.ClinicId == clinicId);
    }

    public async Task<Patient> CreateAsync(Patient patient, Guid clinicId)
    {
        patient.Id = Guid.NewGuid();
        patient.ClinicId = clinicId;
        patient.CreatedAt = DateTime.UtcNow;
        patient.UpdatedAt = DateTime.UtcNow;

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(patient.Id, clinicId) ?? patient;
    }

    public async Task<Patient?> UpdateAsync(Guid id, Guid clinicId, Action<Patient> updateAction)
    {
        var patient = await _db.Patients
            .Include(p => p.Tutor)
            .Include(p => p.Species)
            .Include(p => p.Breed)
            .FirstOrDefaultAsync(p => p.Id == id && p.ClinicId == clinicId);

        if (patient == null) return null;

        updateAction(patient);
        patient.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return patient;
    }

    public async Task<List<MedicalRecord>> GetMedicalRecordsAsync(Guid patientId, Guid clinicId)
    {
        return await _db.MedicalRecords
            .Include(m => m.Veterinarian)
            .Include(m => m.Prescriptions)
            .Where(m => m.PatientId == patientId && m.ClinicId == clinicId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ExamRequest>> GetExamsAsync(Guid patientId, Guid clinicId)
    {
        return await _db.ExamRequests
            .Include(e => e.ExamType)
            .Include(e => e.RequestedBy)
            .Include(e => e.Result)
            .Where(e => e.PatientId == patientId && e.ClinicId == clinicId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Hospitalization>> GetHospitalizationsAsync(Guid patientId, Guid clinicId)
    {
        return await _db.Hospitalizations
            .Include(h => h.Veterinarian)
            .Where(h => h.PatientId == patientId && h.ClinicId == clinicId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }
}
