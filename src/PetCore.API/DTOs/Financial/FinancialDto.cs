namespace PetCore.API.DTOs.Financial;

public class FinancialCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Active { get; set; }
}

public class CreateFinancialCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Revenue ou Expense
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid FinancialCategoryId { get; set; }
    public string FinancialCategoryName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? Discount { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? PaymentMethod { get; set; }

    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }

    public Guid? TutorId { get; set; }
    public string? TutorName { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? HospitalizationId { get; set; }
    public Guid? ExamRequestId { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }

    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<InstallmentDto> Installments { get; set; } = [];
}

public class CreateTransactionRequest
{
    public string Type { get; set; } = string.Empty;
    public Guid FinancialCategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? Discount { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime DueDate { get; set; }

    public Guid? TutorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? HospitalizationId { get; set; }
    public Guid? ExamRequestId { get; set; }
    public Guid? CostCenterId { get; set; }

    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }

    // Parcelamento
    public int? InstallmentCount { get; set; }
}

public class UpdateTransactionRequest
{
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Discount { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? CostCenterId { get; set; }
    public string? Notes { get; set; }
    public string? InvoiceNumber { get; set; }
}

public class PayTransactionRequest
{
    public decimal? AmountPaid { get; set; }
    public string? PaymentMethod { get; set; }
}

public class InstallmentDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CashFlowDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
}

public class FinancialSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalOverdue { get; set; }
    public int TransactionCount { get; set; }
}
