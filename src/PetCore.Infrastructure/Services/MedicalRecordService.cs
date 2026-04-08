using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class MedicalRecordService
{
    private readonly AppDbContext _db;

    public MedicalRecordService(AppDbContext db)
    {
        _db = db;
    }

    private IQueryable<MedicalRecord> BaseQuery(Guid clinicId) =>
        _db.MedicalRecords
            .Include(m => m.Patient)
            .Include(m => m.Veterinarian)
            .Include(m => m.Prescriptions)
            .Where(m => m.ClinicId == clinicId);

    public async Task<(List<MedicalRecord> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize,
        Guid? patientId, Guid? veterinarianId, DateTime? startDate, DateTime? endDate)
    {
        var query = BaseQuery(clinicId);

        if (patientId.HasValue)
            query = query.Where(m => m.PatientId == patientId.Value);

        if (veterinarianId.HasValue)
            query = query.Where(m => m.VeterinarianId == veterinarianId.Value);

        if (startDate.HasValue)
            query = query.Where(m => m.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(m => m.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<MedicalRecord?> GetByIdAsync(Guid id, Guid clinicId)
    {
        return await BaseQuery(clinicId)
            .Include(m => m.ExamRequests).ThenInclude(e => e.ExamType)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MedicalRecord> CreateAsync(MedicalRecord record, Guid clinicId, Guid veterinarianId)
    {
        record.Id = Guid.NewGuid();
        record.ClinicId = clinicId;
        record.VeterinarianId = veterinarianId;
        record.CreatedAt = DateTime.UtcNow;
        record.UpdatedAt = DateTime.UtcNow;

        _db.MedicalRecords.Add(record);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(record.Id, clinicId))!;
    }

    public async Task<MedicalRecord?> UpdateAsync(Guid id, Guid clinicId, Action<MedicalRecord> updateAction)
    {
        var record = await _db.MedicalRecords.FirstOrDefaultAsync(m => m.Id == id && m.ClinicId == clinicId);
        if (record == null) return null;

        updateAction(record);
        record.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetByIdAsync(id, clinicId);
    }

    public async Task<Prescription> AddPrescriptionAsync(Guid medicalRecordId, Guid clinicId, Prescription prescription)
    {
        var record = await _db.MedicalRecords.FirstOrDefaultAsync(m => m.Id == medicalRecordId && m.ClinicId == clinicId)
            ?? throw new InvalidOperationException("Prontuário não encontrado.");

        prescription.Id = Guid.NewGuid();
        prescription.MedicalRecordId = medicalRecordId;
        prescription.CreatedAt = DateTime.UtcNow;

        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync();
        return prescription;
    }

    public async Task<bool> RemovePrescriptionAsync(Guid medicalRecordId, Guid prescriptionId, Guid clinicId)
    {
        var record = await _db.MedicalRecords.FirstOrDefaultAsync(m => m.Id == medicalRecordId && m.ClinicId == clinicId);
        if (record == null) return false;

        var prescription = await _db.Prescriptions
            .FirstOrDefaultAsync(p => p.Id == prescriptionId && p.MedicalRecordId == medicalRecordId);
        if (prescription == null) return false;

        _db.Prescriptions.Remove(prescription);
        await _db.SaveChangesAsync();
        return true;
    }
}
