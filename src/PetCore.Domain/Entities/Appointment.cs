using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? VeterinarianId { get; set; }
    public AppointmentType Type { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public User? Veterinarian { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }
    public ICollection<FinancialTransaction> Transactions { get; set; } = [];
}
