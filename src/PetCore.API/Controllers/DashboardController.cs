using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Dashboard;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboard;
    private readonly ProductService _productService;

    public DashboardController(DashboardService dashboard, ProductService productService)
    {
        _dashboard = dashboard;
        _productService = productService;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var (patients, tutors, appts, hosp, exams, lowStock, rev, exp) =
            await _dashboard.GetSummaryAsync(GetClinicId());

        return Ok(new DashboardSummaryDto
        {
            TotalPatients = patients, TotalTutors = tutors,
            AppointmentsToday = appts, ActiveHospitalizations = hosp,
            PendingExams = exams, LowStockProducts = lowStock,
            MonthlyRevenue = rev, MonthlyExpense = exp
        });
    }

    [HttpGet("appointments-today")]
    public async Task<IActionResult> GetAppointmentsToday()
    {
        var data = await _dashboard.GetAppointmentsTodayAsync(GetClinicId());
        return Ok(data.Select(d => new DashboardAppointmentDto
        {
            Id = d.Id, PatientName = d.PatientName, TutorName = d.TutorName,
            VeterinarianName = d.VetName, Type = d.Type, Status = d.Status,
            ScheduledAt = d.ScheduledAt
        }));
    }

    [HttpGet("financial-summary")]
    public async Task<IActionResult> GetFinancialSummary()
    {
        var (revenue, expense, daily) = await _dashboard.GetFinancialSummaryAsync(GetClinicId());
        return Ok(new DashboardFinancialDto
        {
            TotalRevenue = revenue, TotalExpense = expense, Balance = revenue - expense,
            Daily = daily.Select(d => new DailyFinancialDto
            { Date = d.Date, Revenue = d.Rev, Expense = d.Exp }).ToList()
        });
    }

    [HttpGet("stock-alerts")]
    public async Task<IActionResult> GetStockAlerts()
    {
        var clinicId = GetClinicId();
        var zero = await _productService.GetZeroStockAsync(clinicId);
        var low = await _productService.GetLowStockAsync(clinicId);
        var expiring = await _productService.GetExpiringAsync(clinicId, 30);

        var alerts = zero.Select(p => new DashboardStockAlertDto
        {
            ProductId = p.Id, ProductName = p.Name, CategoryName = p.Category.Name,
            CurrentStock = p.CurrentStock, MinStock = p.MinStock, AlertType = "Zero"
        })
        .Concat(low.Select(p => new DashboardStockAlertDto
        {
            ProductId = p.Id, ProductName = p.Name, CategoryName = p.Category.Name,
            CurrentStock = p.CurrentStock, MinStock = p.MinStock, AlertType = "Low"
        }))
        .Concat(expiring.Select(p => new DashboardStockAlertDto
        {
            ProductId = p.Id, ProductName = p.Name, CategoryName = p.Category.Name,
            CurrentStock = p.CurrentStock, MinStock = p.MinStock,
            AlertType = "Expiring", ExpirationDate = p.ExpirationDate
        }));

        return Ok(alerts);
    }

    [HttpGet("hospitalizations-active")]
    public async Task<IActionResult> GetActiveHospitalizations()
    {
        var (items, _) = await new HospitalizationService(
            HttpContext.RequestServices.GetRequiredService<Infrastructure.Data.AppDbContext>())
            .GetPagedAsync(GetClinicId(), 1, 50, Domain.Enums.HospitalizationStatus.Active, null);

        return Ok(items.Select(h => new
        {
            h.Id, PatientName = h.Patient.Name, TutorName = h.Patient.Tutor.Name,
            VeterinarianName = h.Veterinarian.Name, h.Cage, h.Reason, h.AdmittedAt
        }));
    }

    [HttpGet("top-services")]
    public async Task<IActionResult> GetTopServices()
    {
        var data = await _dashboard.GetTopServicesAsync(GetClinicId());
        return Ok(data.Select(d => new TopServiceDto { ServiceName = d.ServiceName, Count = d.Count }));
    }

    [HttpGet("patients-chart")]
    public async Task<IActionResult> GetPatientsChart()
    {
        var data = await _dashboard.GetPatientsChartAsync(GetClinicId());
        return Ok(data.Select(d => new PatientsChartDto { Date = d.Date, Count = d.Count }));
    }
}
