using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;
using System.Security.Claims;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstadoSesionController : ControllerBase
{
    private readonly IEstadoSesionService _sesionService;

    public EstadoSesionController(IEstadoSesionService sesionService)
    {
        _sesionService = sesionService;
    }

    /// <summary>
    /// Obtiene la lista de usuarios y su estado de actividad actual.
    /// Permite filtrar por rol (p.ej. 'REPARTIDOR', 'ADMIN').
    /// </summary>
    /// <param name="rol">Filtro opcional por rol.</param>
    [HttpGet("activos")]
    [Authorize(Roles = "ADMIN")] // Solo los admins deben ver quién está activo
    public async Task<ActionResult<IEnumerable<ResumenSesionDto>>> ObtenerUsuariosActivos([FromQuery] string? rol = null)
    {
        var activos = await _sesionService.ObtenerUsuariosActivosAsync(rol);
        return Ok(activos);
    }

    /// <summary>
    /// Obtiene el historial de sesiones de un usuario específico.
    /// </summary>
    [HttpGet("historial/{usuarioId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<EstadoSesionDto>>> ObtenerHistorial(int usuarioId)
    {
        var historial = await _sesionService.ObtenerHistorialSesionesAsync(usuarioId);
        return Ok(historial);
    }

    /// <summary>
    /// Obtiene el historial del propio usuario autenticado.
    /// </summary>
    [HttpGet("mis-sesiones")]
    public async Task<ActionResult<IEnumerable<EstadoSesionDto>>> ObtenerMisSesiones()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var historial = await _sesionService.ObtenerHistorialSesionesAsync(userId);
        return Ok(historial);
    }
}
