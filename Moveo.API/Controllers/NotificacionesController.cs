using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;

namespace Moveo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _notificacionService;

    public NotificacionesController(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerPorUsuarioId(int usuarioId)
    {
        return Ok(await _notificacionService.ObtenerPorUsuarioIdAsync(usuarioId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificacionDto>> ObtenerPorId(int id)
    {
        var notificacion = await _notificacionService.ObtenerPorIdAsync(id);
        if (notificacion == null) return NotFound();
        return Ok(notificacion);
    }

    [HttpPost]
    public async Task<ActionResult<NotificacionDto>> Crear(CrearNotificacionDto dto)
    {
        var notificacion = await _notificacionService.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = notificacion.Id }, notificacion);
    }

    [HttpPut("{id}/leida")]
    public async Task<ActionResult> MarcarComoLeida(int id)
    {
        await _notificacionService.MarcarComoLeidaAsync(id);
        return NoContent();
    }
}
