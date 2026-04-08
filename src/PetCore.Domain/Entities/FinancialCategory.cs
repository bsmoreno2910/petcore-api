using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class FinancialCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
