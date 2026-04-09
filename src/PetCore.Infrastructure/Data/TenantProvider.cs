using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PetCore.Infrastructure.Data;

public interface ITenantProvider
{
    Guid? ClinicaId { get; }
    Guid? UsuarioId { get; }
    bool EhSuperAdmin { get; }
}

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _accessor;

    public TenantProvider(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? ClinicaId
    {
        get
        {
            var claim = _accessor.HttpContext?.User?.FindFirstValue("clinicaId");
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public Guid? UsuarioId
    {
        get
        {
            var claim = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public bool EhSuperAdmin =>
        _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) == "SuperAdmin";
}
