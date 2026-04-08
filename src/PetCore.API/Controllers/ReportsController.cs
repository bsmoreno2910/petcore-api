using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
public class ReportsController : ControllerBase
{
    private readonly ExcelExportService _excel;

    public ReportsController(ExcelExportService excel)
    {
        _excel = excel;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;
    private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? "Sistema";
    private string GetClinicName() => "PetCore"; // Simplified; could load from DB

    private FileContentResult Excel(byte[] bytes, string filename) =>
        File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);

    [HttpGet("inventory")]
    public async Task<IActionResult> Inventory([FromQuery] Guid? categoryId, [FromQuery] string? stockStatus)
    {
        var bytes = await _excel.ExportInventoryAsync(GetClinicId(), GetClinicName(), GetUserName(), categoryId, stockStatus);
        return Excel(bytes, "PetCore_Inventario.xlsx");
    }

    [HttpGet("stock-movements")]
    public async Task<IActionResult> StockMovements(
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
        [FromQuery] MovementType? type, [FromQuery] Guid? productId)
    {
        var bytes = await _excel.ExportStockMovementsAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate, type, productId);
        return Excel(bytes, "PetCore_Movimentacoes.xlsx");
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> Appointments(
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
        [FromQuery] AppointmentType? type, [FromQuery] Guid? veterinarianId)
    {
        var bytes = await _excel.ExportAppointmentsAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate, type, veterinarianId);
        return Excel(bytes, "PetCore_Atendimentos.xlsx");
    }

    [HttpGet("patients")]
    public async Task<IActionResult> Patients([FromQuery] Guid? speciesId, [FromQuery] bool? active)
    {
        var bytes = await _excel.ExportPatientsAsync(GetClinicId(), GetClinicName(), GetUserName(), speciesId, active);
        return Excel(bytes, "PetCore_Pacientes.xlsx");
    }

    [HttpGet("tutors")]
    public async Task<IActionResult> Tutors([FromQuery] bool? active)
    {
        var bytes = await _excel.ExportTutorsAsync(GetClinicId(), GetClinicName(), GetUserName(), active);
        return Excel(bytes, "PetCore_Tutores.xlsx");
    }

    [HttpGet("hospitalizations")]
    public async Task<IActionResult> Hospitalizations(
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] HospitalizationStatus? status)
    {
        var bytes = await _excel.ExportHospitalizationsAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate, status);
        return Excel(bytes, "PetCore_Internacoes.xlsx");
    }

    [HttpGet("exams")]
    public async Task<IActionResult> Exams(
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
        [FromQuery] Guid? examTypeId, [FromQuery] ExamStatus? status)
    {
        var bytes = await _excel.ExportExamsAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate, examTypeId, status);
        return Excel(bytes, "PetCore_Exames.xlsx");
    }

    [HttpGet("financial-revenue")]
    public async Task<IActionResult> Revenue(
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] Guid? categoryId)
    {
        var bytes = await _excel.ExportFinancialRevenueAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate, categoryId);
        return Excel(bytes, "PetCore_Receitas.xlsx");
    }

    [HttpGet("financial-expenses")]
    public async Task<IActionResult> Expenses(
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] Guid? categoryId)
    {
        var bytes = await _excel.ExportFinancialExpensesAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate, categoryId);
        return Excel(bytes, "PetCore_Despesas.xlsx");
    }

    [HttpGet("financial-cashflow")]
    public async Task<IActionResult> CashFlow([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var bytes = await _excel.ExportCashFlowAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate);
        return Excel(bytes, "PetCore_FluxoCaixa.xlsx");
    }

    [HttpGet("financial-overdue")]
    public async Task<IActionResult> Overdue()
    {
        var bytes = await _excel.ExportOverdueAsync(GetClinicId(), GetClinicName(), GetUserName());
        return Excel(bytes, "PetCore_Inadimplencia.xlsx");
    }

    [HttpGet("financial-by-category")]
    public async Task<IActionResult> ByCategory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var bytes = await _excel.ExportByFinancialCategoryAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate);
        return Excel(bytes, "PetCore_PorCategoria.xlsx");
    }

    [HttpGet("financial-by-tutor")]
    public async Task<IActionResult> ByTutor([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var bytes = await _excel.ExportByTutorAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate);
        return Excel(bytes, "PetCore_PorTutor.xlsx");
    }

    [HttpGet("cost-centers")]
    public async Task<IActionResult> CostCenters([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var bytes = await _excel.ExportCostCentersAsync(GetClinicId(), GetClinicName(), GetUserName(), startDate, endDate);
        return Excel(bytes, "PetCore_CentrosCusto.xlsx");
    }
}
