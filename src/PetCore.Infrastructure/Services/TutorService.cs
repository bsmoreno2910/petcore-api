using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class TutorService
{
    private readonly AppDbContext _db;

    public TutorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Tutor> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize, string? search, string? phone, string? cpf)
    {
        var query = _db.Tutors
            .Include(t => t.Patients)
            .Where(t => t.ClinicId == clinicId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t =>
                t.Name.Contains(search) ||
                (t.Email != null && t.Email.Contains(search)) ||
                (t.Cpf != null && t.Cpf.Contains(search)) ||
                (t.Rg != null && t.Rg.Contains(search)) ||
                (t.Phone != null && t.Phone.Contains(search)));

        if (!string.IsNullOrWhiteSpace(phone))
            query = query.Where(t => t.Phone != null && t.Phone.Contains(phone));

        if (!string.IsNullOrWhiteSpace(cpf))
            query = query.Where(t => t.Cpf != null && t.Cpf.Contains(cpf));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Tutor?> GetByIdAsync(Guid id, Guid clinicId)
    {
        return await _db.Tutors
            .Include(t => t.Patients)
                .ThenInclude(p => p.Species)
            .Include(t => t.Patients)
                .ThenInclude(p => p.Breed)
            .FirstOrDefaultAsync(t => t.Id == id && t.ClinicId == clinicId);
    }

    public async Task<Tutor> CreateAsync(Tutor tutor, Guid clinicId)
    {
        tutor.Id = Guid.NewGuid();
        tutor.ClinicId = clinicId;
        tutor.CreatedAt = DateTime.UtcNow;
        tutor.UpdatedAt = DateTime.UtcNow;

        _db.Tutors.Add(tutor);
        await _db.SaveChangesAsync();
        return tutor;
    }

    public async Task<Tutor?> UpdateAsync(Guid id, Guid clinicId, Action<Tutor> updateAction)
    {
        var tutor = await _db.Tutors.FirstOrDefaultAsync(t => t.Id == id && t.ClinicId == clinicId);
        if (tutor == null) return null;

        updateAction(tutor);
        tutor.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return tutor;
    }

    public async Task<List<Patient>> GetPatientsAsync(Guid tutorId, Guid clinicId)
    {
        return await _db.Patients
            .Include(p => p.Species)
            .Include(p => p.Breed)
            .Where(p => p.TutorId == tutorId && p.ClinicId == clinicId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<(decimal TotalRevenue, decimal TotalPaid, decimal TotalPending, decimal TotalOverdue)>
        GetFinancialSummaryAsync(Guid tutorId, Guid clinicId)
    {
        var transactions = await _db.FinancialTransactions
            .Where(t => t.TutorId == tutorId && t.ClinicId == clinicId && t.Type == TransactionType.Revenue)
            .ToListAsync();

        var totalRevenue = transactions.Sum(t => t.Amount);
        var totalPaid = transactions.Where(t => t.Status == TransactionStatus.Paid).Sum(t => t.AmountPaid ?? t.Amount);
        var totalPending = transactions.Where(t => t.Status == TransactionStatus.Pending).Sum(t => t.Amount);
        var totalOverdue = transactions.Where(t => t.Status == TransactionStatus.Overdue).Sum(t => t.Amount);

        return (totalRevenue, totalPaid, totalPending, totalOverdue);
    }
}
