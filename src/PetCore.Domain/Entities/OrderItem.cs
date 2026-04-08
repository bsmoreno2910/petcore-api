namespace PetCore.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int QuantityRequested { get; set; }
    public int? QuantityApproved { get; set; }
    public int QuantityReceived { get; set; }
    public string? Notes { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
