using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class ExamService
{
    private readonly AppDbContext _db;

    public ExamService(AppDbContext db)
    {
        _db = db;
    }

    // --- ExamTypes (globais) ---

    public async Task<List<ExamType>> GetExamTypesAsync()
    {
        return await _db.ExamTypes.OrderBy(e => e.Name).ToListAsync();
    }

    public async Task<ExamType> CreateExamTypeAsync(string name, string? category, decimal? defaultPrice)
    {
        var examType = new ExamType
        {
            Id = Guid.NewGuid(),
            Name = name,
            Category = category,
            DefaultPrice = defaultPrice,
            CreatedAt = DateTime.UtcNow
        };

        _db.ExamTypes.Add(examType);
        await _db.SaveChangesAsync();
        return examType;
    }

    public async Task<ExamType?> UpdateExamTypeAsync(Guid id, string name, string? category, decimal? defaultPrice)
    {
        var examType = await _db.ExamTypes.FindAsync(id);
        if (examType == null) return null;

        examType.Name = name;
        examType.Category = category;
        examType.DefaultPrice = defaultPrice;
        await _db.SaveChangesAsync();
        return examType;
    }

    // --- ExamRequests ---

    private IQueryable<ExamRequest> BaseQuery(Guid clinicId) =>
        _db.ExamRequests
            .Include(e => e.Patient).ThenInclude(p => p.Tutor)
            .Include(e => e.RequestedBy)
            .Include(e => e.ExamType)
            .Include(e => e.Result).ThenInclude(r => r!.PerformedBy)
            .Where(e => e.ClinicId == clinicId);

    public async Task<(List<ExamRequest> Items, int TotalCount)> GetExamRequestsPagedAsync(
        Guid clinicId, int page, int pageSize,
        ExamStatus? status, Guid? patientId, Guid? examTypeId,
        DateTime? startDate, DateTime? endDate)
    {
        var query = BaseQuery(clinicId);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);
        if (patientId.HasValue)
            query = query.Where(e => e.PatientId == patientId.Value);
        if (examTypeId.HasValue)
            query = query.Where(e => e.ExamTypeId == examTypeId.Value);
        if (startDate.HasValue)
            query = query.Where(e => e.RequestedAt >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(e => e.RequestedAt <= endDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<ExamRequest?> GetExamRequestByIdAsync(Guid id, Guid clinicId)
    {
        return await BaseQuery(clinicId).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<ExamRequest> CreateExamRequestAsync(
        Guid clinicId, Guid requestedById, Guid patientId, Guid examTypeId,
        Guid? medicalRecordId, string? clinicalIndication, string? notes)
    {
        var examRequest = new ExamRequest
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            PatientId = patientId,
            RequestedById = requestedById,
            ExamTypeId = examTypeId,
            MedicalRecordId = medicalRecordId,
            ClinicalIndication = clinicalIndication,
            Notes = notes,
            Status = ExamStatus.Requested,
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.ExamRequests.Add(examRequest);
        await _db.SaveChangesAsync();

        return (await GetExamRequestByIdAsync(examRequest.Id, clinicId))!;
    }

    public async Task<ExamRequest?> CollectAsync(Guid id, Guid clinicId)
    {
        var exam = await _db.ExamRequests.FirstOrDefaultAsync(e => e.Id == id && e.ClinicId == clinicId);
        if (exam == null) return null;

        if (exam.Status != ExamStatus.Requested)
            throw new InvalidOperationException("Apenas exames solicitados podem ter coleta registrada.");

        exam.Status = ExamStatus.SampleCollected;
        exam.CollectedAt = DateTime.UtcNow;
        exam.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetExamRequestByIdAsync(id, clinicId);
    }

    public async Task<ExamRequest?> CancelAsync(Guid id, Guid clinicId)
    {
        var exam = await _db.ExamRequests.FirstOrDefaultAsync(e => e.Id == id && e.ClinicId == clinicId);
        if (exam == null) return null;

        if (exam.Status == ExamStatus.Completed || exam.Status == ExamStatus.Cancelled)
            throw new InvalidOperationException("Exame já finalizado ou cancelado.");

        exam.Status = ExamStatus.Cancelled;
        exam.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await GetExamRequestByIdAsync(id, clinicId);
    }

    public async Task<ExamResult> AddResultAsync(
        Guid examRequestId, Guid clinicId, Guid performedById,
        string? resultText, string? resultFileUrl, string? referenceValues,
        string? observations, string? conclusion)
    {
        var exam = await _db.ExamRequests.FirstOrDefaultAsync(e => e.Id == examRequestId && e.ClinicId == clinicId)
            ?? throw new InvalidOperationException("Solicitação de exame não encontrada.");

        if (exam.Status == ExamStatus.Completed)
            throw new InvalidOperationException("Exame já possui resultado.");

        if (exam.Status == ExamStatus.Cancelled)
            throw new InvalidOperationException("Exame cancelado não pode receber resultado.");

        var result = new ExamResult
        {
            Id = Guid.NewGuid(),
            ExamRequestId = examRequestId,
            PerformedById = performedById,
            ResultText = resultText,
            ResultFileUrl = resultFileUrl,
            ReferenceValues = referenceValues,
            Observations = observations,
            Conclusion = conclusion,
            CreatedAt = DateTime.UtcNow
        };

        exam.Status = ExamStatus.Completed;
        exam.CompletedAt = DateTime.UtcNow;
        exam.UpdatedAt = DateTime.UtcNow;

        _db.ExamResults.Add(result);
        await _db.SaveChangesAsync();

        return await _db.ExamResults
            .Include(r => r.PerformedBy)
            .FirstAsync(r => r.Id == result.Id);
    }

    public async Task<ExamResult?> GetResultAsync(Guid examRequestId, Guid clinicId)
    {
        var exam = await _db.ExamRequests.FirstOrDefaultAsync(e => e.Id == examRequestId && e.ClinicId == clinicId);
        if (exam == null) return null;

        return await _db.ExamResults
            .Include(r => r.PerformedBy)
            .FirstOrDefaultAsync(r => r.ExamRequestId == examRequestId);
    }
}
