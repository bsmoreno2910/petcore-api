namespace PetCore.API.DTOs.Exams;

public class ExamTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal? DefaultPrice { get; set; }
    public bool Active { get; set; }
}

public class CreateExamTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal? DefaultPrice { get; set; }
}

public class UpdateExamTypeRequest : CreateExamTypeRequest { }

public class ExamRequestDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TutorName { get; set; } = string.Empty;
    public Guid RequestedById { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public Guid ExamTypeId { get; set; }
    public string ExamTypeName { get; set; } = string.Empty;
    public string? ExamTypeCategory { get; set; }
    public Guid? MedicalRecordId { get; set; }

    public string Status { get; set; } = string.Empty;
    public string? ClinicalIndication { get; set; }
    public string? Notes { get; set; }

    public DateTime RequestedAt { get; set; }
    public DateTime? CollectedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public ExamResultDto? Result { get; set; }
}

public class CreateExamRequestDto
{
    public Guid PatientId { get; set; }
    public Guid ExamTypeId { get; set; }
    public Guid? MedicalRecordId { get; set; }
    public string? ClinicalIndication { get; set; }
    public string? Notes { get; set; }
}

public class ExamResultDto
{
    public Guid Id { get; set; }
    public Guid ExamRequestId { get; set; }
    public Guid PerformedById { get; set; }
    public string PerformedByName { get; set; } = string.Empty;

    public string? ResultText { get; set; }
    public string? ResultFileUrl { get; set; }
    public string? ReferenceValues { get; set; }
    public string? Observations { get; set; }
    public string? Conclusion { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateExamResultRequest
{
    public string? ResultText { get; set; }
    public string? ResultFileUrl { get; set; }
    public string? ReferenceValues { get; set; }
    public string? Observations { get; set; }
    public string? Conclusion { get; set; }
}
