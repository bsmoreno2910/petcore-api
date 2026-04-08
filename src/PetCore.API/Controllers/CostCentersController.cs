using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.CostCenters;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/cost-centers")]
[Authorize]
public class CostCentersController : ControllerBase
{
    private readonly CostCenterService _service;
    private readonly IMapper _mapper;

    public CostCentersController(CostCenterService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetAll()
    {
        var centers = await _service.GetAllAsync(GetClinicId());
        return Ok(_mapper.Map<List<CostCenterDto>>(centers));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Financial")]
    public async Task<IActionResult> Create([FromBody] CreateCostCenterRequest request)
    {
        var cc = await _service.CreateAsync(GetClinicId(), request.Name, request.Description);
        return Created("/api/cost-centers", _mapper.Map<CostCenterDto>(cc));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCostCenterRequest request)
    {
        var cc = await _service.UpdateAsync(id, GetClinicId(), request.Name, request.Description);
        return cc == null ? NotFound() : Ok(_mapper.Map<CostCenterDto>(cc));
    }

    [HttpGet("{id:guid}/summary")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var (rev, exp, count) = await _service.GetSummaryAsync(id, GetClinicId());
        return Ok(new CostCenterSummaryDto
        {
            CostCenterId = id,
            TotalRevenue = rev,
            TotalExpense = exp,
            Balance = rev - exp,
            TransactionCount = count
        });
    }

    [HttpGet("report")]
    [Authorize(Roles = "SuperAdmin,Admin,Financial,Viewer")]
    public async Task<IActionResult> GetReport()
    {
        var data = await _service.GetReportAsync(GetClinicId());
        return Ok(data.Select(d => new CostCenterSummaryDto
        {
            CostCenterId = d.CostCenterId,
            CostCenterName = d.Name,
            TotalRevenue = d.TotalRevenue,
            TotalExpense = d.TotalExpense,
            Balance = d.TotalRevenue - d.TotalExpense,
            TransactionCount = d.Count
        }));
    }
}
