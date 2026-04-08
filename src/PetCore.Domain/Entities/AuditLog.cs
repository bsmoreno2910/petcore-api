namespace PetCore.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
