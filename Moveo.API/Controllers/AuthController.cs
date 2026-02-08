using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión de autenticación, registro y sesiones de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<RespuestaAuthDto>> Registrar(RegistroUsuarioDto registerDto)
    {
        try
        {
            var result = await _authService.RegistrarAsync(registerDto);
            SetTokenCookie(result.TokenDeAcceso, "X-Access-Token", 15);
            SetTokenCookie(result.TokenDeRefresco, "X-Refresh-Token", 10080); // 7 días
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("iniciar-sesion")]
    public async Task<ActionResult<RespuestaAuthDto>> IniciarSesion(LoginUsuarioDto loginDto)
    {
        try
        {
            var result = await _authService.IniciarSesionAsync(loginDto);
            SetTokenCookie(result.TokenDeAcceso, "X-Access-Token", 15);
            SetTokenCookie(result.TokenDeRefresco, "X-Refresh-Token", 10080); // 7 días
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("refrescar-token")]
    public async Task<ActionResult<RespuestaAuthDto>> RefrescarToken(SolicitudRefrescarTokenDto requestDto)
    {
        try
        {
            var tokenDeRefresco = requestDto.TokenDeRefresco;

            // Si no viene en el body, lo buscamos en la cookie
            if (string.IsNullOrEmpty(tokenDeRefresco))
            {
                tokenDeRefresco = Request.Cookies["X-Refresh-Token"];
            }

            if (string.IsNullOrEmpty(tokenDeRefresco))
            {
                return BadRequest("No se proporcionó el token de refresco");
            }

            var result = await _authService.RefrescarTokenAsync(tokenDeRefresco);
            SetTokenCookie(result.TokenDeAcceso, "X-Access-Token", 15);
            SetTokenCookie(result.TokenDeRefresco, "X-Refresh-Token", 10080); // 7 días
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("cerrar-sesion")]
    public async Task<IActionResult> CerrarSesion(SolicitudCerrarSesionDto solicitudDto)
    {
        try
        {
            var tokenDeRefresco = solicitudDto.TokenDeRefresco;

            if (string.IsNullOrEmpty(tokenDeRefresco))
            {
                tokenDeRefresco = Request.Cookies["X-Refresh-Token"];
            }

            if (!string.IsNullOrEmpty(tokenDeRefresco))
            {
                await _authService.CerrarSesionAsync(tokenDeRefresco);
            }

            // Limpiar cookies
            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token");

            return Ok(new { message = "Sesión cerrada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private void SetTokenCookie(string token, string name, int expireMinutes)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
            Secure = true, // En desarrollo se puede poner false si no hay HTTPS, pero recomendable true
            SameSite = SameSiteMode.Lax, // Lax es más compatible para navegación entre front y back
            Path = "/" // Disponible en toda la app
        };

        Response.Cookies.Append(name, token, cookieOptions);
    }
}
