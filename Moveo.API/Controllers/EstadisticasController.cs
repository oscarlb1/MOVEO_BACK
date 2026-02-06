using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

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

    [HttpGet("ranking")]
    [AllowAnonymous] // El ranking suele ser público o visible para todos
    public async Task<ActionResult<IEnumerable<RankingUsuarioDto>>> ObtenerRanking([FromQuery] int count = 5, [FromQuery] string? filtro = null, [FromQuery] string? sortBy = "entregas")
    {
        var ranking = await _estadisticaService.ObtenerRankingTopAsync(count, filtro, sortBy);
        return Ok(ranking);
    }

    [HttpGet("global")]
    public async Task<ActionResult<EstadisticaGlobalDto>> ObtenerEstadisticasGlobales()
    {
        var globalStats = await _estadisticaService.ObtenerEstadisticasGlobalesAsync();
        return Ok(globalStats);
    }

    [HttpGet("usuario/{id}")]
    public async Task<ActionResult<EstadisticaUsuarioDto>> ObtenerPorUsuarioId(int id)
    {
        var stats = await _estadisticaService.ObtenerEstadisticasUsuarioAsync(id);
        if (stats == null) return NotFound($"Estadísticas no encontradas para el usuario con ID {id}.");

        return Ok(stats);
    }
}
