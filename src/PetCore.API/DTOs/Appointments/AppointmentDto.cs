namespace PetCore.API.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TutorName { get; set; } = string.Empty;
    public string? TutorPhone { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public Guid? VeterinarianId { get; set; }
    public string? VeterinarianName { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAppointmentRequest
{
    public Guid PatientId { get; set; }
    public Guid? VeterinarianId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAppointmentRequest
{
    public Guid? VeterinarianId { get; set; }
    public string? Type { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class CancelAppointmentRequest
{
    public string? CancellationReason { get; set; }
}

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Color { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? VeterinarianId { get; set; }
}

public class AvailableSlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
