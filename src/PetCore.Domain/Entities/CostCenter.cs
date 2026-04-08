namespace PetCore.Domain.Entities;

public class CostCenter
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public ICollection<FinancialTransaction> Transactions { get; set; } = [];
}
