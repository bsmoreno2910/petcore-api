using PetCore.Domain.Entities;
using PetCore.Domain.Interfaces;
using PetCore.Infrastructure.Data;

namespace PetCore.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;

    public AuditService(AppDbContext db)
    {
        _db = db;
    }

    public async Task LogAsync(Guid clinicId, Guid userId, string action, string entity, string entityId,
        string? oldValue = null, string? newValue = null, string? ipAddress = null)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            UserId = userId,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
