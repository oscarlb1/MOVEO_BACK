using System.Security.Claims;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Middleware;

public class UserActivityMiddleware
{
    private readonly RequestDelegate _next;

    public UserActivityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IEstadoSesionService sesionService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                await sesionService.RegistrarActividadAsync(userId);
            }
        }

        await _next(context);
    }
}

public static class UserActivityMiddlewareExtensions
{
    public static IApplicationBuilder UseUserActivity(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserActivityMiddleware>();
    }
}
