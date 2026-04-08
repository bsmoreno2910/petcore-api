using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class TransactionInstallment
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    public FinancialTransaction Transaction { get; set; } = null!;
}
