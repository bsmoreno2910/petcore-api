using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Products;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/product-categories")]
[Authorize]
public class ProductCategoriesController : ControllerBase
{
    private readonly ProductService _service;
    private readonly IMapper _mapper;

    public ProductCategoriesController(ProductService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetCategoriesAsync();
        return Ok(_mapper.Map<List<ProductCategoryDto>>(categories));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductCategoryRequest request)
    {
        var cat = await _service.CreateCategoryAsync(request.Name, request.Description, request.Color);
        return Created("/api/product-categories", _mapper.Map<ProductCategoryDto>(cat));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductCategoryRequest request)
    {
        var cat = await _service.UpdateCategoryAsync(id, request.Name, request.Description, request.Color);
        return cat == null ? NotFound() : Ok(_mapper.Map<ProductCategoryDto>(cat));
    }
}
