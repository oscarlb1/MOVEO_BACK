using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

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
            var result = await _authService.RefrescarTokenAsync(requestDto.TokenDeRefresco);
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
            await _authService.CerrarSesionAsync(solicitudDto.TokenDeRefresco);
            return Ok(new { message = "Sesión cerrada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
