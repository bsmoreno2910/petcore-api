namespace PetCore.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<AuthResult> SelectClinicAsync(Guid userId, Guid clinicId);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public object? User { get; set; }
    public object? Clinics { get; set; }
    public string? Error { get; set; }
}
