using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Orders;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;
    private readonly ExcelExportService _excel;
    private readonly IMapper _mapper;

    public OrdersController(OrderService service, ExcelExportService excel, IMapper mapper)
    {
        _service = service;
        _excel = excel;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;
    private Guid GetUserId() => (Guid)HttpContext.Items["UserId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _service.GetAllAsync(GetClinicId());
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _service.GetByIdAsync(id, GetClinicId());
        return order == null ? NotFound() : Ok(_mapper.Map<OrderDto>(order));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var type = Enum.TryParse<OrderType>(request.Type, true, out var t) ? t : OrderType.Regular;

        var items = request.Items
            .Select(i => (i.ProductId, i.QuantityRequested, i.Notes))
            .ToList();

        var order = await _service.CreateAsync(
            GetClinicId(), GetUserId(), type,
            request.Period, request.Notes, request.Justification, items);

        var dto = _mapper.Map<OrderDto>(order);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request)
    {
        try
        {
            var items = request.Items?
                .Select(i => (i.ProductId, i.QuantityRequested, i.Notes))
                .ToList();

            var order = await _service.UpdateAsync(
                id, GetClinicId(), request.Period, request.Notes, request.Justification, items);

            return order == null ? NotFound() : Ok(_mapper.Map<OrderDto>(order));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/submit")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> Submit(Guid id)
    {
        try
        {
            var order = await _service.SubmitAsync(id, GetClinicId());
            return order == null ? NotFound() : Ok(_mapper.Map<OrderDto>(order));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/approve")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Approve(Guid id)
    {
        try
        {
            var order = await _service.ApproveAsync(id, GetClinicId(), GetUserId());
            return order == null ? NotFound() : Ok(_mapper.Map<OrderDto>(order));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/receive")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceiveOrderRequest request)
    {
        try
        {
            var items = request.Items
                .Select(i => (i.OrderItemId, i.QuantityReceived))
                .ToList();

            var order = await _service.ReceiveAsync(id, GetClinicId(), GetUserId(), items);
            return order == null ? NotFound() : Ok(_mapper.Map<OrderDto>(order));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var order = await _service.CancelAsync(id, GetClinicId());
            return order == null ? NotFound() : Ok(_mapper.Map<OrderDto>(order));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/export")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<IActionResult> Export(Guid id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Sistema";
        var bytes = await _excel.ExportOrderAsync(id, GetClinicId(), "PetCore", userName);
        if (bytes.Length == 0) return NotFound();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PetCore_Pedido_{id:N}.xlsx");
    }
}
