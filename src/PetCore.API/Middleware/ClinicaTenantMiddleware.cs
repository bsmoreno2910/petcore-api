using System.Security.Claims;

namespace PetCore.API.Middleware;

public class ClinicaTenantMiddleware
{
    private readonly RequestDelegate _next;

    public ClinicaTenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var clinicaIdClaim = context.User.FindFirstValue("clinicaId");
            if (Guid.TryParse(clinicaIdClaim, out var clinicaId))
                context.Items["ClinicaId"] = clinicaId;

            var usuarioIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(usuarioIdClaim, out var usuarioId))
                context.Items["UsuarioId"] = usuarioId;

            var perfilClaim = context.User.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(perfilClaim))
                context.Items["Perfil"] = perfilClaim;
        }

        await _next(context);
    }
}

public static class ClinicaTenantMiddlewareExtensions
{
    public static IApplicationBuilder UseClinicaTenant(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ClinicaTenantMiddleware>();
}
