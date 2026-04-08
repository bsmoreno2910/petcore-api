using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class FinancialTransaction
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public Guid FinancialCategoryId { get; set; }

    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? AmountPaid { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }

    public Guid? TutorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? HospitalizationId { get; set; }
    public Guid? ExamRequestId { get; set; }
    public Guid? CostCenterId { get; set; }

    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }

    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public FinancialCategory FinancialCategory { get; set; } = null!;
    public Tutor? Tutor { get; set; }
    public Appointment? Appointment { get; set; }
    public Hospitalization? Hospitalization { get; set; }
    public ExamRequest? ExamRequest { get; set; }
    public CostCenter? CostCenter { get; set; }
    public User CreatedBy { get; set; } = null!;
    public ICollection<TransactionInstallment> Installments { get; set; } = [];
}
