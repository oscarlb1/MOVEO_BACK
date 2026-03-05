using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moveo.Modelos.DTOs;
using Moveo.Negocio.Servicios;
using System.Security.Claims;

namespace Moveo.API.Controllers;

/// <summary>
/// Controlador para la gestión de notificaciones de usuario (leídas, no leídas, avisos administrativos).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _notificacionService;

    public NotificacionesController(INotificacionService notificacionService)
    {
        _notificacionService = notificacionService;
    }

    /// <summary>
    /// Obtiene las notificaciones del usuario autenticado.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerMisNotificaciones()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notificaciones = await _notificacionService.ObtenerMisNotificacionesAsync(userId);
        return Ok(notificaciones);
    }

    /// <summary>
    /// Obtiene el conteo de notificaciones no leídas del usuario.
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> ObtenerConteoNoLeidas()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var count = await _notificacionService.ObtenerConteoNoLeidasAsync(userId);
        return Ok(count);
    }

    /// <summary>
    /// Obtiene todas las notificaciones (Solo Administrador).
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerTodasAdmin()
    {
        var notificaciones = await _notificacionService.ObtenerTodasAdminAsync();
        return Ok(notificaciones);
    }

    /// <summary>
    /// Envía una notificación a un usuario específico (Solo Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Enviar(CrearNotificacionDto dto)
    {
        await _notificacionService.EnviarNotificacionAsync(dto);
        return Ok(new { message = "Notificación enviada correctamente" });
    }

    /// <summary>
    /// Envía una notificación a todos los usuarios (Solo Administrador).
    /// </summary>
    [HttpPost("broadcast")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> EnviarBroadcast(CrearNotificacionDto dto)
    {
        await _notificacionService.EnviarABroadcastAsync(dto);
        return Ok(new { message = "Broadcast enviado correctamente" });
    }

    /// <summary>
    /// Permite a un repartidor enviar una incidencia global o al administrador.
    /// </summary>
    [HttpPost("incidencia")]
    [Authorize(Roles = "REPARTIDOR")]
    public async Task<IActionResult> ReportarIncidencia(CrearNotificacionDto dto)
    {
        // Usamos el Título para indicar que es una incidencia
        if (string.IsNullOrEmpty(dto.Titulo)) 
        {
            dto.Titulo = "INCIDENCIA GLOBAL";
        }
        else 
        {
            dto.Titulo = "INCIDENCIA: " + dto.Titulo;
        }
        
        // Si el usuario reporta, broadcast informa a todos.
        await _notificacionService.EnviarABroadcastAsync(dto);
        return Ok(new { message = "Incidencia reportada correctamente" });
    }

    /// <summary>
    /// Marca una notificación como leída.
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        var success = await _notificacionService.MarcarComoLeidaAsync(id);
        if (!success) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Marca todas las notificaciones del usuario como leídas.
    /// </summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarcarTodasLeidas()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _notificacionService.MarcarTodasComoLeidasAsync(userId);
        return Ok();
    }

    /// <summary>
    /// Elimina una notificación.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var success = await _notificacionService.EliminarNotificacionAsync(id);
        if (!success) return NotFound();
        return Ok();
    }
}
