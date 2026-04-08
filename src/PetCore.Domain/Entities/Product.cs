namespace PetCore.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
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
    public bool Active { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public ProductCategory Category { get; set; } = null!;
    public ProductUnit Unit { get; set; } = null!;
    public ICollection<Movement> Movements { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
