namespace PetCore.Domain.Interfaces;

public interface IAuditService
{
    Task LogAsync(Guid clinicId, Guid userId, string action, string entity, string entityId,
        string? oldValue = null, string? newValue = null, string? ipAddress = null);
}
