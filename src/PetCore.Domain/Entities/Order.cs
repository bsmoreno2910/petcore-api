using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Code { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public string? Period { get; set; }
    public string? Notes { get; set; }
    public string? Justification { get; set; }

    public Guid CreatedById { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public User? ApprovedBy { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
    public ICollection<Movement> Movements { get; set; } = [];
}
