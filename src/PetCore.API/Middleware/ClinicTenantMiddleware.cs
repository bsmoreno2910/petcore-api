using System.Security.Claims;

namespace PetCore.API.Middleware;

public class ClinicTenantMiddleware
{
    private readonly RequestDelegate _next;

    public ClinicTenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var clinicIdClaim = context.User.FindFirstValue("clinicId");
            if (!string.IsNullOrEmpty(clinicIdClaim))
            {
                context.Items["ClinicId"] = Guid.Parse(clinicIdClaim);
            }

            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                context.Items["UserId"] = Guid.Parse(userIdClaim);
            }

            var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(roleClaim))
            {
                context.Items["UserRole"] = roleClaim;
            }
        }

        await _next(context);
    }
}

public static class ClinicTenantMiddlewareExtensions
{
    public static IApplicationBuilder UseClinicTenant(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClinicTenantMiddleware>();
    }
}
