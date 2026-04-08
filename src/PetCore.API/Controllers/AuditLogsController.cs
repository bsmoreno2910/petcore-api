using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCore.API.DTOs.Common;
using PetCore.Infrastructure.Data;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ExcelExportService _excel;

    public AuditLogsController(AppDbContext db, ExcelExportService excel)
    {
        _db = db;
        _excel = excel;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? entity = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _db.AuditLogs
            .Include(a => a.User)
            .Where(a => a.ClinicId == GetClinicId());

        if (userId.HasValue) query = query.Where(a => a.UserId == userId.Value);
        if (!string.IsNullOrEmpty(entity)) query = query.Where(a => a.Entity == entity);
        if (!string.IsNullOrEmpty(action)) query = query.Where(a => a.Action == action);
        if (startDate.HasValue) query = query.Where(a => a.CreatedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(a => a.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id, a.UserId, UserName = a.User.Name,
                a.Action, a.Entity, a.EntityId,
                a.OldValue, a.NewValue, a.IpAddress, a.CreatedAt
            })
            .ToListAsync();

        return Ok(new { items, totalCount, page, pageSize });
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] Guid? userId = null,
        [FromQuery] string? entity = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Sistema";
        var bytes = await _excel.ExportAuditLogsAsync(
            GetClinicId(), "PetCore", userName, userId, entity, action, startDate, endDate);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PetCore_AuditLogs.xlsx");
    }
}
