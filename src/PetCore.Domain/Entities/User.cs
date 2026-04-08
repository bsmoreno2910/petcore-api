namespace PetCore.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Crmv { get; set; }
    public string? AvatarUrl { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<ClinicUser> ClinicUsers { get; set; } = [];
    public ICollection<Movement> MovementsCreated { get; set; } = [];
    public ICollection<Movement> MovementsApproved { get; set; } = [];
    public ICollection<Order> OrdersCreated { get; set; } = [];
    public ICollection<Order> OrdersApproved { get; set; } = [];
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = [];
    public ICollection<Appointment> AppointmentsAsVet { get; set; } = [];
    public ICollection<AuditLog> AuditLogs { get; set; } = [];
}
