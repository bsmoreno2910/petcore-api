using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Entities;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class FinancialService
{
    private readonly AppDbContext _db;

    public FinancialService(AppDbContext db)
    {
        _db = db;
    }

    // --- Categories ---
    public async Task<List<FinancialCategory>> GetCategoriesAsync() =>
        await _db.FinancialCategories.OrderBy(c => c.Name).ToListAsync();

    public async Task<FinancialCategory> CreateCategoryAsync(string name, TransactionType type)
    {
        var cat = new FinancialCategory
        {
            Id = Guid.NewGuid(), Name = name, Type = type, CreatedAt = DateTime.UtcNow
        };
        _db.FinancialCategories.Add(cat);
        await _db.SaveChangesAsync();
        return cat;
    }

    // --- Transactions ---
    private IQueryable<FinancialTransaction> BaseQuery(Guid clinicId) =>
        _db.FinancialTransactions
            .Include(t => t.FinancialCategory)
            .Include(t => t.Tutor)
            .Include(t => t.CostCenter)
            .Include(t => t.CreatedBy)
            .Include(t => t.Installments)
            .Where(t => t.ClinicId == clinicId);

    public async Task<(List<FinancialTransaction> Items, int TotalCount)> GetPagedAsync(
        Guid clinicId, int page, int pageSize,
        TransactionType? type, TransactionStatus? status, Guid? categoryId, Guid? tutorId,
        DateTime? startDate, DateTime? endDate)
    {
        var query = BaseQuery(clinicId);

        if (type.HasValue) query = query.Where(t => t.Type == type.Value);
        if (status.HasValue) query = query.Where(t => t.Status == status.Value);
        if (categoryId.HasValue) query = query.Where(t => t.FinancialCategoryId == categoryId.Value);
        if (tutorId.HasValue) query = query.Where(t => t.TutorId == tutorId.Value);
        if (startDate.HasValue) query = query.Where(t => t.DueDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.DueDate <= endDate.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.DueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<FinancialTransaction?> GetByIdAsync(Guid id, Guid clinicId) =>
        await BaseQuery(clinicId).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<FinancialTransaction> CreateAsync(
        Guid clinicId, Guid userId, TransactionType type, Guid financialCategoryId,
        string description, decimal amount, decimal? discount, PaymentMethod? paymentMethod,
        DateTime dueDate, Guid? tutorId, Guid? appointmentId, Guid? hospitalizationId,
        Guid? examRequestId, Guid? costCenterId, string? notes, string? invoiceNumber,
        int? installmentCount)
    {
        var transaction = new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            Type = type,
            Status = TransactionStatus.Pending,
            FinancialCategoryId = financialCategoryId,
            Description = description,
            Amount = amount,
            Discount = discount,
            PaymentMethod = paymentMethod,
            DueDate = dueDate,
            TutorId = tutorId,
            AppointmentId = appointmentId,
            HospitalizationId = hospitalizationId,
            ExamRequestId = examRequestId,
            CostCenterId = costCenterId,
            Notes = notes,
            InvoiceNumber = invoiceNumber,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.FinancialTransactions.Add(transaction);

        // Gerar parcelas se solicitado
        if (installmentCount.HasValue && installmentCount.Value > 1)
        {
            var netAmount = amount - (discount ?? 0);
            var installmentAmount = Math.Round(netAmount / installmentCount.Value, 2);

            for (int i = 1; i <= installmentCount.Value; i++)
            {
                _db.TransactionInstallments.Add(new TransactionInstallment
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    InstallmentNumber = i,
                    Amount = installmentAmount,
                    DueDate = dueDate.AddMonths(i - 1),
                    Status = TransactionStatus.Pending
                });
            }
        }

        await _db.SaveChangesAsync();
        return (await GetByIdAsync(transaction.Id, clinicId))!;
    }

    public async Task<FinancialTransaction?> UpdateAsync(Guid id, Guid clinicId, Action<FinancialTransaction> updateAction)
    {
        var transaction = await _db.FinancialTransactions.FirstOrDefaultAsync(t => t.Id == id && t.ClinicId == clinicId);
        if (transaction == null) return null;

        updateAction(transaction);
        transaction.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<FinancialTransaction?> PayAsync(Guid id, Guid clinicId, decimal? amountPaid, PaymentMethod? paymentMethod)
    {
        var transaction = await _db.FinancialTransactions
            .Include(t => t.Installments)
            .FirstOrDefaultAsync(t => t.Id == id && t.ClinicId == clinicId);

        if (transaction == null) return null;

        var netAmount = transaction.Amount - (transaction.Discount ?? 0);
        transaction.AmountPaid = amountPaid ?? netAmount;
        transaction.PaymentMethod = paymentMethod ?? transaction.PaymentMethod;
        transaction.PaidAt = DateTime.UtcNow;
        transaction.Status = transaction.AmountPaid >= netAmount
            ? TransactionStatus.Paid
            : TransactionStatus.PartiallyPaid;
        transaction.UpdatedAt = DateTime.UtcNow;

        // Se não tem parcelas, marcar como pago direto
        if (!transaction.Installments.Any())
        {
            await _db.SaveChangesAsync();
            return await GetByIdAsync(id, clinicId);
        }

        // Se tem parcelas, pagar todas pendentes
        foreach (var inst in transaction.Installments.Where(i => i.Status == TransactionStatus.Pending))
        {
            inst.PaidAt = DateTime.UtcNow;
            inst.Status = TransactionStatus.Paid;
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<FinancialTransaction?> CancelAsync(Guid id, Guid clinicId)
    {
        var transaction = await _db.FinancialTransactions
            .Include(t => t.Installments)
            .FirstOrDefaultAsync(t => t.Id == id && t.ClinicId == clinicId);

        if (transaction == null) return null;

        transaction.Status = TransactionStatus.Cancelled;
        transaction.UpdatedAt = DateTime.UtcNow;

        foreach (var inst in transaction.Installments)
            inst.Status = TransactionStatus.Cancelled;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id, clinicId);
    }

    public async Task<TransactionInstallment?> PayInstallmentAsync(Guid installmentId)
    {
        var inst = await _db.TransactionInstallments
            .Include(i => i.Transaction)
            .FirstOrDefaultAsync(i => i.Id == installmentId);

        if (inst == null) return null;

        inst.PaidAt = DateTime.UtcNow;
        inst.Status = TransactionStatus.Paid;

        // Verificar se todas as parcelas foram pagas
        var allPaid = await _db.TransactionInstallments
            .Where(i => i.TransactionId == inst.TransactionId && i.Id != installmentId)
            .AllAsync(i => i.Status == TransactionStatus.Paid);

        if (allPaid)
        {
            inst.Transaction.Status = TransactionStatus.Paid;
            inst.Transaction.PaidAt = DateTime.UtcNow;
            inst.Transaction.AmountPaid = inst.Transaction.Amount - (inst.Transaction.Discount ?? 0);
        }
        else
        {
            inst.Transaction.Status = TransactionStatus.PartiallyPaid;
        }

        inst.Transaction.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return inst;
    }

    // --- Cash Flow ---
    public async Task<List<(DateTime Date, decimal Revenue, decimal Expense)>> GetCashFlowAsync(
        Guid clinicId, DateTime startDate, DateTime endDate)
    {
        var transactions = await _db.FinancialTransactions
            .Where(t => t.ClinicId == clinicId && t.Status == TransactionStatus.Paid &&
                        t.PaidAt >= startDate && t.PaidAt <= endDate)
            .ToListAsync();

        return transactions
            .GroupBy(t => t.PaidAt!.Value.Date)
            .Select(g => (
                Date: g.Key,
                Revenue: g.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.AmountPaid ?? t.Amount),
                Expense: g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.AmountPaid ?? t.Amount)
            ))
            .OrderBy(x => x.Date)
            .ToList();
    }

    // --- Summary ---
    public async Task<(decimal TotalRevenue, decimal TotalExpense, decimal TotalPending, decimal TotalOverdue, int Count)>
        GetSummaryAsync(Guid clinicId)
    {
        var transactions = await _db.FinancialTransactions
            .Where(t => t.ClinicId == clinicId && t.Status != TransactionStatus.Cancelled)
            .ToListAsync();

        var totalRevenue = transactions.Where(t => t.Type == TransactionType.Revenue && t.Status == TransactionStatus.Paid)
            .Sum(t => t.AmountPaid ?? t.Amount);
        var totalExpense = transactions.Where(t => t.Type == TransactionType.Expense && t.Status == TransactionStatus.Paid)
            .Sum(t => t.AmountPaid ?? t.Amount);
        var totalPending = transactions.Where(t => t.Status == TransactionStatus.Pending).Sum(t => t.Amount);
        var totalOverdue = transactions.Where(t => t.Status == TransactionStatus.Overdue).Sum(t => t.Amount);

        return (totalRevenue, totalExpense, totalPending, totalOverdue, transactions.Count);
    }

    // --- Overdue ---
    public async Task<List<FinancialTransaction>> GetOverdueAsync(Guid clinicId) =>
        await BaseQuery(clinicId)
            .Where(t => t.Status == TransactionStatus.Pending && t.DueDate < DateTime.UtcNow)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
}
