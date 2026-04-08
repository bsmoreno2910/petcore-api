using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Movement
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid ProductId { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }

    public Guid CreatedById { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Product Product { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public User? ApprovedBy { get; set; }
    public Order? Order { get; set; }
}
