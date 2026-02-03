using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasUsuarioController : ControllerBase
{
    private readonly IEstadisticaUsuarioService _estadisticaUsuarioService;

    public EstadisticasUsuarioController(IEstadisticaUsuarioService estadisticaUsuarioService)
    {
        _estadisticaUsuarioService = estadisticaUsuarioService;
    }

    [HttpGet("{usuarioId}")]
    public async Task<ActionResult<EstadisticaUsuarioDto>> ObtenerPorUsuarioId(int usuarioId)
    {
        var estadistica = await _estadisticaUsuarioService.ObtenerPorUsuarioIdAsync(usuarioId);
        if (estadistica == null) return NotFound();
        return Ok(estadistica);
    }

    [HttpPost("{usuarioId}")]
    public async Task<ActionResult<EstadisticaUsuarioDto>> CrearOActualizar(int usuarioId, ActualizarEstadisticaUsuarioDto dto)
    {
        var estadistica = await _estadisticaUsuarioService.CrearOActualizarAsync(usuarioId, dto);
        return Ok(estadistica);
    }
}
