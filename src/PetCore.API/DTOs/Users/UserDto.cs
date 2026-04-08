namespace PetCore.API.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Crmv { get; set; }
    public string? AvatarUrl { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserClinicDto> Clinics { get; set; } = [];
}

public class UserClinicDto
{
    public Guid ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Crmv { get; set; }
}

public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Crmv { get; set; }
    public string? AvatarUrl { get; set; }
}
