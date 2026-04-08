using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Financial;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/financial")]
[Authorize]
public class FinancialTransactionsController : ControllerBase
{
    private readonly FinancialService _service;
    private readonly IMapper _mapper;

    public FinancialTransactionsController(FinancialService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;
    private Guid GetUserId() => (Guid)HttpContext.Items["UserId"]!;

    [HttpGet("transactions")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TransactionType? type = null,
        [FromQuery] TransactionStatus? status = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? tutorId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var (items, totalCount) = await _service.GetPagedAsync(
            GetClinicId(), page, pageSize, type, status, categoryId, tutorId, startDate, endDate);

        return Ok(new PagedResponse<TransactionDto>
        {
            Items = _mapper.Map<List<TransactionDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("transactions/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _service.GetByIdAsync(id, GetClinicId());
        return transaction == null ? NotFound() : Ok(_mapper.Map<TransactionDto>(transaction));
    }

    [HttpPost("transactions")]
    [Authorize(Roles = "SuperAdmin,Admin,Receptionist,Financial")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        if (!Enum.TryParse<TransactionType>(request.Type, true, out var type))
            return BadRequest(new { error = "Tipo inválido." });

        Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var pm);

        var transaction = await _service.CreateAsync(
            GetClinicId(), GetUserId(), type, request.FinancialCategoryId,
            request.Description, request.Amount, request.Discount,
            string.IsNullOrEmpty(request.PaymentMethod) ? null : pm,
            request.DueDate, request.TutorId, request.AppointmentId,
            request.HospitalizationId, request.ExamRequestId, request.CostCenterId,
            request.Notes, request.InvoiceNumber, request.InstallmentCount);

        var dto = _mapper.Map<TransactionDto>(transaction);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("transactions/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionRequest request)
    {
        var transaction = await _service.UpdateAsync(id, GetClinicId(), t =>
        {
            if (request.Description != null) t.Description = request.Description;
            if (request.Amount.HasValue) t.Amount = request.Amount.Value;
            if (request.Discount.HasValue) t.Discount = request.Discount;
            if (request.DueDate.HasValue) t.DueDate = request.DueDate.Value;
            if (request.CostCenterId.HasValue) t.CostCenterId = request.CostCenterId;
            if (request.Notes != null) t.Notes = request.Notes;
            if (request.InvoiceNumber != null) t.InvoiceNumber = request.InvoiceNumber;
        });
        return transaction == null ? NotFound() : Ok(_mapper.Map<TransactionDto>(transaction));
    }

    [HttpPatch("transactions/{id:guid}/pay")]
    [Authorize(Roles = "SuperAdmin,Admin,Receptionist,Financial")]
    public async Task<IActionResult> Pay(Guid id, [FromBody] PayTransactionRequest request)
    {
        Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var pm);
        var transaction = await _service.PayAsync(
            id, GetClinicId(), request.AmountPaid,
            string.IsNullOrEmpty(request.PaymentMethod) ? null : pm);
        return transaction == null ? NotFound() : Ok(_mapper.Map<TransactionDto>(transaction));
    }

    [HttpPatch("transactions/{id:guid}/cancel")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var transaction = await _service.CancelAsync(id, GetClinicId());
        return transaction == null ? NotFound() : Ok(_mapper.Map<TransactionDto>(transaction));
    }

    [HttpGet("cash-flow")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetCashFlow(
        [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var data = await _service.GetCashFlowAsync(GetClinicId(), startDate, endDate);
        var cumulative = 0m;
        return Ok(data.Select(d =>
        {
            cumulative += d.Revenue - d.Expense;
            return new CashFlowDto
            {
                Date = d.Date, Revenue = d.Revenue, Expense = d.Expense, Balance = cumulative
            };
        }));
    }

    [HttpGet("summary")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetSummary()
    {
        var (rev, exp, pending, overdue, count) = await _service.GetSummaryAsync(GetClinicId());
        return Ok(new FinancialSummaryDto
        {
            TotalRevenue = rev, TotalExpense = exp, Balance = rev - exp,
            TotalPending = pending, TotalOverdue = overdue, TransactionCount = count
        });
    }

    [HttpGet("overdue")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetOverdue()
    {
        var transactions = await _service.GetOverdueAsync(GetClinicId());
        return Ok(_mapper.Map<List<TransactionDto>>(transactions));
    }

    [HttpPatch("installments/{id:guid}/pay")]
    [Authorize(Roles = "SuperAdmin,Admin,Receptionist,Financial")]
    public async Task<IActionResult> PayInstallment(Guid id)
    {
        var inst = await _service.PayInstallmentAsync(id);
        return inst == null ? NotFound() : Ok(_mapper.Map<InstallmentDto>(inst));
    }
}
