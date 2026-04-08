using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCore.API.DTOs.Common;
using PetCore.API.DTOs.Products;
using PetCore.Domain.Entities;
using PetCore.Infrastructure.Services;

namespace PetCore.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;
    private readonly ExcelExportService _excel;
    private readonly IMapper _mapper;

    public ProductsController(ProductService service, ExcelExportService excel, IMapper mapper)
    {
        _service = service;
        _excel = excel;
        _mapper = mapper;
    }

    private Guid GetClinicId() => (Guid)HttpContext.Items["ClinicId"]!;

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Operator,Viewer")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? stockStatus = null)
    {
        var (items, totalCount) = await _service.GetPagedAsync(GetClinicId(), page, pageSize, search, categoryId, stockStatus);
        return Ok(new PagedResponse<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin,Veterinarian,Operator,Viewer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id, GetClinicId());
        return product == null ? NotFound() : Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            CategoryId = request.CategoryId,
            UnitId = request.UnitId,
            Name = request.Name,
            Presentation = request.Presentation,
            CurrentStock = request.CurrentStock,
            MinStock = request.MinStock,
            MaxStock = request.MaxStock,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            Location = request.Location,
            Barcode = request.Barcode,
            Batch = request.Batch,
            ExpirationDate = request.ExpirationDate,
            Notes = request.Notes
        };

        var created = await _service.CreateAsync(product, GetClinicId());
        var dto = _mapper.Map<ProductDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _service.UpdateAsync(id, GetClinicId(), p =>
        {
            p.CategoryId = request.CategoryId;
            p.UnitId = request.UnitId;
            p.Name = request.Name;
            p.Presentation = request.Presentation;
            p.MinStock = request.MinStock;
            p.MaxStock = request.MaxStock;
            p.CostPrice = request.CostPrice;
            p.SellingPrice = request.SellingPrice;
            p.Location = request.Location;
            p.Barcode = request.Barcode;
            p.Batch = request.Batch;
            p.ExpirationDate = request.ExpirationDate;
            p.Notes = request.Notes;
        });
        return product == null ? NotFound() : Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id, GetClinicId());
        return result ? NoContent() : NotFound();
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator,Viewer")]
    public async Task<IActionResult> GetLowStock()
    {
        var products = await _service.GetLowStockAsync(GetClinicId());
        return Ok(_mapper.Map<List<ProductDto>>(products));
    }

    [HttpGet("zero-stock")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator,Viewer")]
    public async Task<IActionResult> GetZeroStock()
    {
        var products = await _service.GetZeroStockAsync(GetClinicId());
        return Ok(_mapper.Map<List<ProductDto>>(products));
    }

    [HttpGet("expiring")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator,Viewer")]
    public async Task<IActionResult> GetExpiring([FromQuery] int daysAhead = 30)
    {
        var products = await _service.GetExpiringAsync(GetClinicId(), daysAhead);
        return Ok(_mapper.Map<List<ProductDto>>(products));
    }

    [HttpGet("export")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Export([FromQuery] string? search = null, [FromQuery] Guid? categoryId = null)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Sistema";
        var bytes = await _excel.ExportProductsAsync(GetClinicId(), "PetCore", userName, search, categoryId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PetCore_Produtos.xlsx");
    }
}
