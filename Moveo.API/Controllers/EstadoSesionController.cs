using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadoSesionController : ControllerBase
{
    private readonly IEstadoSesionService _estadoSesionService;

    public EstadoSesionController(IEstadoSesionService estadoSesionService)
    {
        _estadoSesionService = estadoSesionService;
    }

    [HttpGet("{usuarioId}")]
    public async Task<ActionResult<EstadoSesionDto>> ObtenerPorUsuarioId(int usuarioId)
    {
        var estado = await _estadoSesionService.ObtenerPorUsuarioIdAsync(usuarioId);
        if (estado == null) return NotFound();
        return Ok(estado);
    }

    [HttpPut("{usuarioId}")]
    public async Task<ActionResult<EstadoSesionDto>> ActualizarEstado(int usuarioId, ActualizarEstadoSesionDto dto)
    {
        var estado = await _estadoSesionService.ActualizarEstadoAsync(usuarioId, dto);
        return Ok(estado);
    }
}
