using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class CostCenterService
{
    private readonly AppDbContext _db;

    public CostCenterService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CostCenter>> GetAllAsync(Guid clinicId) =>
        await _db.CostCenters
            .Where(c => c.ClinicId == clinicId)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<CostCenter> CreateAsync(Guid clinicId, string name, string? description)
    {
        var cc = new CostCenter
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
        _db.CostCenters.Add(cc);
        await _db.SaveChangesAsync();
        return cc;
    }

    public async Task<CostCenter?> UpdateAsync(Guid id, Guid clinicId, string name, string? description)
    {
        var cc = await _db.CostCenters.FirstOrDefaultAsync(c => c.Id == id && c.ClinicId == clinicId);
        if (cc == null) return null;
        cc.Name = name;
        cc.Description = description;
        await _db.SaveChangesAsync();
        return cc;
    }

    public async Task<(decimal TotalRevenue, decimal TotalExpense, int Count)> GetSummaryAsync(Guid id, Guid clinicId)
    {
        var transactions = await _db.FinancialTransactions
            .Where(t => t.CostCenterId == id && t.ClinicId == clinicId && t.Status != TransactionStatus.Cancelled)
            .ToListAsync();

        var rev = transactions.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.Amount);
        var exp = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        return (rev, exp, transactions.Count);
    }

    public async Task<List<(Guid CostCenterId, string Name, decimal TotalRevenue, decimal TotalExpense, int Count)>>
        GetReportAsync(Guid clinicId)
    {
        var centers = await _db.CostCenters
            .Where(c => c.ClinicId == clinicId)
            .Include(c => c.Transactions)
            .ToListAsync();

        return centers.Select(c =>
        {
            var active = c.Transactions.Where(t => t.Status != TransactionStatus.Cancelled).ToList();
            return (
                c.Id,
                c.Name,
                active.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.Amount),
                active.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                active.Count
            );
        }).ToList();
    }
}
