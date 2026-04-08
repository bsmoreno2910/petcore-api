using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Movements;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/movements")]
[Authorize]
public class MovementsController : ControllerBase
{
    private readonly MovementService _service;
    private readonly ExcelExportService _excel;
    private readonly IMapper _mapper;

    public MovementsController(MovementService service, ExcelExportService excel, IMapper mapper)
    {
        _service = service;
        _excel = excel;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;
    private Guid GetUserId() => (Guid)HttpContext.Items["UserId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Operator,Viewer")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] MovementType? type = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (items, totalCount) = await _service.GetPagedAsync(
            GetClinicId(), page, pageSize, type, productId, userId, startDate, endDate);

        return Ok(new PagedResponse<MovementDto>
        {
            Items = _mapper.Map<List<MovementDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator,Viewer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var movement = await _service.GetByIdAsync(id, GetClinicId());
        return movement == null ? NotFound() : Ok(_mapper.Map<MovementDto>(movement));
    }

    [HttpPost("entry")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public Task<IActionResult> CreateEntry([FromBody] CreateMovementRequest request)
        => CreateMovement(request, (cs, uid, pid, qty, r, n) => cs.CreateEntryAsync(GetClinicId(), uid, pid, qty, r, n));

    [HttpPost("exit")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public Task<IActionResult> CreateExit([FromBody] CreateMovementRequest request)
        => CreateMovement(request, (cs, uid, pid, qty, r, n) => cs.CreateExitAsync(GetClinicId(), uid, pid, qty, r, n));

    [HttpPost("adjustment")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public Task<IActionResult> CreateAdjustment([FromBody] CreateMovementRequest request)
        => CreateMovement(request, (cs, uid, pid, qty, r, n) => cs.CreateAdjustmentAsync(GetClinicId(), uid, pid, qty, r, n));

    [HttpPost("loss")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public Task<IActionResult> CreateLoss([FromBody] CreateMovementRequest request)
        => CreateMovement(request, (cs, uid, pid, qty, r, n) => cs.CreateLossAsync(GetClinicId(), uid, pid, qty, r, n));

    private async Task<IActionResult> CreateMovement(
        CreateMovementRequest request,
        Func<MovementService, Guid, Guid, int, string?, string?, Task<Domain.Entities.Movement>> factory)
    {
        try
        {
            var movement = await factory(
                _service, GetUserId(), request.ProductId, request.Quantity, request.Reason, request.Notes);
            return Created($"/api/movements/{movement.Id}", _mapper.Map<MovementDto>(movement));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("export")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> Export(
        [FromQuery] MovementType? type = null, [FromQuery] Guid? productId = null,
        [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Sistema";
        var bytes = await _excel.ExportStockMovementsAsync(
            GetClinicId(), "PetCore", userName, startDate, endDate, type, productId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PetCore_Movimentacoes.xlsx");
    }
}
