using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Products;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/product-units")]
[Authorize]
public class ProductUnitsController : ControllerBase
{
    private readonly ProductService _service;
    private readonly IMapper _mapper;

    public ProductUnitsController(ProductService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units = await _service.GetUnitsAsync();
        return Ok(_mapper.Map<List<ProductUnitDto>>(units));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductUnitRequest request)
    {
        var unit = await _service.CreateUnitAsync(request.Abbreviation, request.Name);
        return Created("/api/product-units", _mapper.Map<ProductUnitDto>(unit));
    }
}
