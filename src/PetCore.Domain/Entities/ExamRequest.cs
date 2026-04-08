using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class ExamRequest
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid RequestedById { get; set; }
    public Guid ExamTypeId { get; set; }
    public Guid? MedicalRecordId { get; set; }

    public ExamStatus Status { get; set; } = ExamStatus.Requested;
    public string? ClinicalIndication { get; set; }
    public string? Notes { get; set; }

    public DateTime RequestedAt { get; set; }
    public DateTime? CollectedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Patient Patient { get; set; } = null!;
    public User RequestedBy { get; set; } = null!;
    public ExamType ExamType { get; set; } = null!;
    public MedicalRecord? MedicalRecord { get; set; }
    public ExamResult? Result { get; set; }
    public ICollection<FinancialTransaction> Transactions { get; set; } = [];
}
