using PetCore.Domain.Enums;

namespace PetCore.Domain.Entities;

public class ClinicUser
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Clinic Clinic { get; set; } = null!;
    public User User { get; set; } = null!;
}
