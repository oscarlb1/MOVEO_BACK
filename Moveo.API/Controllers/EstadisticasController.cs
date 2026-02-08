using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión y consulta de estadísticas de usuarios y del sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstadisticasController : ControllerBase
{
    private readonly IEstadisticaService _estadisticaService;

    public EstadisticasController(IEstadisticaService estadisticaService)
    {
        _estadisticaService = estadisticaService;
    }

    /// <summary>
    /// Obtiene las estadísticas de rendimiento y entregas del usuario autenticado.
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<EstadisticaUsuarioDto>> ObtenerMisEstadisticas()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("No se pudo identificar al usuario.");
        }

        var stats = await _estadisticaService.ObtenerEstadisticasUsuarioAsync(userId);
        if (stats == null) return NotFound("Estadísticas no encontradas para este usuario.");

        return Ok(stats);
    }

    /// <summary>
    /// Obtiene el ranking de usuarios basado en entregas u otros criterios.
    /// </summary>
    /// <param name="count">Número de usuarios a mostrar en el ranking.</param>
    /// <param name="filtro">Filtro opcional (p.ej. por período).</param>
    /// <param name="sortBy">Criterio de ordenación.</param>
    [HttpGet("ranking")]
    [AllowAnonymous] // El ranking suele ser público o visible para todos
    public async Task<ActionResult<IEnumerable<RankingUsuarioDto>>> ObtenerRanking([FromQuery] int count = 5, [FromQuery] string? filtro = null, [FromQuery] string? sortBy = "entregas")
    {
        var ranking = await _estadisticaService.ObtenerRankingTopAsync(count, filtro, sortBy);
        return Ok(ranking);
    }

    /// <summary>
    /// Obtiene las estadísticas globales del sistema (Solo Administrador).
    /// </summary>
    [HttpGet("global")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<EstadisticaGlobalDto>> ObtenerEstadisticasGlobales()
    {
        var globalStats = await _estadisticaService.ObtenerEstadisticasGlobalesAsync();
        return Ok(globalStats);
    }

    /// <summary>
    /// Obtiene las estadísticas de un usuario específico por su ID (Solo Administrador).
    /// </summary>
    [HttpGet("usuario/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<EstadisticaUsuarioDto>> ObtenerPorUsuarioId(int id)
    {
        var stats = await _estadisticaService.ObtenerEstadisticasUsuarioAsync(id);
        if (stats == null) return NotFound($"Estadísticas no encontradas para el usuario con ID {id}.");

        return Ok(stats);
    }
}
