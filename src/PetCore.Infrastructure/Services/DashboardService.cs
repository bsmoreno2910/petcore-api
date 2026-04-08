using Microsoft.EntityFrameworkCore;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(int Patients, int Tutors, int AppointmentsToday, int ActiveHosp,
        int PendingExams, int LowStock, decimal MonthRev, decimal MonthExp)>
        GetSummaryAsync(Guid clinicId)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var patients = await _db.Patients.CountAsync(p => p.ClinicId == clinicId && p.Active);
        var tutors = await _db.Tutors.CountAsync(t => t.ClinicId == clinicId && t.Active);

        var appointmentsToday = await _db.Appointments.CountAsync(a =>
            a.ClinicId == clinicId && a.ScheduledAt >= today && a.ScheduledAt < today.AddDays(1));

        var activeHosp = await _db.Hospitalizations.CountAsync(h =>
            h.ClinicId == clinicId && h.Status == HospitalizationStatus.Active);

        var pendingExams = await _db.ExamRequests.CountAsync(e =>
            e.ClinicId == clinicId && (e.Status == ExamStatus.Requested || e.Status == ExamStatus.SampleCollected));

        var lowStock = await _db.Products.CountAsync(p =>
            p.ClinicId == clinicId && p.Active && p.CurrentStock <= p.MinStock);

        var monthTransactions = await _db.FinancialTransactions
            .Where(t => t.ClinicId == clinicId && t.Status == TransactionStatus.Paid &&
                        t.PaidAt >= monthStart && t.PaidAt < monthEnd)
            .ToListAsync();

        var monthRev = monthTransactions.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.AmountPaid ?? t.Amount);
        var monthExp = monthTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.AmountPaid ?? t.Amount);

        return (patients, tutors, appointmentsToday, activeHosp, pendingExams, lowStock, monthRev, monthExp);
    }

    public async Task<List<(Guid Id, string PatientName, string TutorName, string? VetName,
        string Type, string Status, DateTime ScheduledAt)>>
        GetAppointmentsTodayAsync(Guid clinicId)
    {
        var today = DateTime.UtcNow.Date;
        return await _db.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.Tutor)
            .Include(a => a.Veterinarian)
            .Where(a => a.ClinicId == clinicId && a.ScheduledAt >= today && a.ScheduledAt < today.AddDays(1))
            .OrderBy(a => a.ScheduledAt)
            .Select(a => new ValueTuple<Guid, string, string, string?, string, string, DateTime>(
                a.Id, a.Patient.Name, a.Patient.Tutor.Name,
                a.Veterinarian != null ? a.Veterinarian.Name : null,
                a.Type.ToString(), a.Status.ToString(), a.ScheduledAt))
            .ToListAsync();
    }

    public async Task<(decimal Revenue, decimal Expense, List<(DateTime Date, decimal Rev, decimal Exp)> Daily)>
        GetFinancialSummaryAsync(Guid clinicId)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var transactions = await _db.FinancialTransactions
            .Where(t => t.ClinicId == clinicId && t.Status == TransactionStatus.Paid &&
                        t.PaidAt >= monthStart && t.PaidAt < monthEnd)
            .ToListAsync();

        var revenue = transactions.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.AmountPaid ?? t.Amount);
        var expense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.AmountPaid ?? t.Amount);

        var daily = transactions
            .GroupBy(t => t.PaidAt!.Value.Date)
            .Select(g => (
                Date: g.Key,
                Rev: g.Where(t => t.Type == TransactionType.Revenue).Sum(t => t.AmountPaid ?? t.Amount),
                Exp: g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.AmountPaid ?? t.Amount)))
            .OrderBy(x => x.Date)
            .ToList();

        return (revenue, expense, daily);
    }

    public async Task<List<(string ServiceName, int Count)>> GetTopServicesAsync(Guid clinicId)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return await _db.Appointments
            .Where(a => a.ClinicId == clinicId && a.Status == AppointmentStatus.Completed &&
                        a.FinishedAt >= monthStart)
            .GroupBy(a => a.Type)
            .Select(g => new ValueTuple<string, int>(g.Key.ToString(), g.Count()))
            .OrderByDescending(x => x.Item2)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<(DateTime Date, int Count)>> GetPatientsChartAsync(Guid clinicId)
    {
        var start = DateTime.UtcNow.AddDays(-30).Date;
        return await _db.Appointments
            .Where(a => a.ClinicId == clinicId && a.Status == AppointmentStatus.Completed &&
                        a.FinishedAt >= start)
            .GroupBy(a => a.FinishedAt!.Value.Date)
            .Select(g => new ValueTuple<DateTime, int>(g.Key, g.Count()))
            .OrderBy(x => x.Item1)
            .ToListAsync();
    }
}
