namespace PetCore.API.DTOs.MedicalRecords;

public class MedicalRecordDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid VeterinarianId { get; set; }
    public string VeterinarianName { get; set; } = string.Empty;
    public Guid? AppointmentId { get; set; }

    public string? ChiefComplaint { get; set; }
    public string? History { get; set; }
    public string? Anamnesis { get; set; }

    public decimal? Weight { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public string? PhysicalExam { get; set; }
    public string? Mucous { get; set; }
    public string? Hydration { get; set; }
    public string? Lymph { get; set; }

    public string? Diagnosis { get; set; }
    public string? DifferentialDiagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<PrescriptionDto> Prescriptions { get; set; } = [];
}

public class CreateMedicalRecordRequest
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }

    public string? ChiefComplaint { get; set; }
    public string? History { get; set; }
    public string? Anamnesis { get; set; }

    public decimal? Weight { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public int? RespiratoryRate { get; set; }
    public string? PhysicalExam { get; set; }
    public string? Mucous { get; set; }
    public string? Hydration { get; set; }
    public string? Lymph { get; set; }

    public string? Diagnosis { get; set; }
    public string? DifferentialDiagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
}

public class UpdateMedicalRecordRequest : CreateMedicalRecordRequest { }

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public Guid MedicalRecordId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Route { get; set; }
    public string? Instructions { get; set; }
    public int? Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePrescriptionRequest
{
    public string MedicineName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Route { get; set; }
    public string? Instructions { get; set; }
    public int? Quantity { get; set; }
}
