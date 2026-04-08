namespace PetCore.Domain.Entities;

public class MedicalRecord
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid VeterinarianId { get; set; }
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

    public Patient Patient { get; set; } = null!;
    public User Veterinarian { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; } = [];
    public ICollection<ExamRequest> ExamRequests { get; set; } = [];
}
