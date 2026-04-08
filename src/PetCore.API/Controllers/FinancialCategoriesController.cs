using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Financial;
using PetCore.Domain.Enums;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/financial/categories")]
[Authorize]
public class FinancialCategoriesController : ControllerBase
{
    private readonly FinancialService _service;
    private readonly IMapper _mapper;

    public FinancialCategoriesController(FinancialService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetCategoriesAsync();
        return Ok(_mapper.Map<List<FinancialCategoryDto>>(categories));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Financial")]
    public async Task<IActionResult> Create([FromBody] CreateFinancialCategoryRequest request)
    {
        if (!Enum.TryParse<TransactionType>(request.Type, true, out var type))
            return BadRequest(new { error = "Tipo inválido. Use Revenue ou Expense." });

        var cat = await _service.CreateCategoryAsync(request.Name, type);
        return Created("/api/financial/categories", _mapper.Map<FinancialCategoryDto>(cat));
    }
}
