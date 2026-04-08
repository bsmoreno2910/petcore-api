namespace PetCore.API.DTOs.Products;

public class ProductCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool Active { get; set; }
}

public class CreateProductCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class ProductUnitDto
{
    public Guid Id { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateProductUnitRequest
{
    public string Abbreviation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategoryColor { get; set; }
    public Guid UnitId { get; set; }
    public string UnitAbbreviation { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? Presentation { get; set; }
    public int CurrentStock { get; set; }
    public int MinStock { get; set; }
    public int? MaxStock { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string? Location { get; set; }
    public string? Barcode { get; set; }
    public string? Batch { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool Active { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }

    public string StockStatus => CurrentStock <= 0 ? "Zero" :
        CurrentStock <= MinStock ? "Low" : "Ok";
}

public class CreateProductRequest
{
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Presentation { get; set; }
    public int CurrentStock { get; set; }
    public int MinStock { get; set; }
    public int? MaxStock { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string? Location { get; set; }
    public string? Barcode { get; set; }
    public string? Batch { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateProductRequest : CreateProductRequest { }
