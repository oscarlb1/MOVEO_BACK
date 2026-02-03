using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;

    public NotificacionService(INotificacionRepository notificacionRepository)
    {
        _notificacionRepository = notificacionRepository;
    }

    public async Task<IEnumerable<NotificacionDto>> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        return notificaciones.Select(n => new NotificacionDto
        {
            Id = n.Id,
            UsuarioId = n.UsuarioId,
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            Leido = n.Leido,
            Fecha = n.Fecha
        });
    }

    public async Task<NotificacionDto?> ObtenerPorIdAsync(int id)
    {
        var n = await _notificacionRepository.ObtenerPorIdAsync(id);
        if (n == null) return null;

        return new NotificacionDto
        {
            Id = n.Id,
            UsuarioId = n.UsuarioId,
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            Leido = n.Leido,
            Fecha = n.Fecha
        };
    }

    public async Task<NotificacionDto> CrearAsync(CrearNotificacionDto dto)
    {
        var notificacion = new Notificacion
        {
            UsuarioId = dto.UsuarioId,
            Titulo = dto.Titulo,
            Mensaje = dto.Mensaje,
            Leido = false,
            Fecha = DateTime.UtcNow
        };

        await _notificacionRepository.AgregarAsync(notificacion);

        return new NotificacionDto
        {
            Id = notificacion.Id,
            UsuarioId = notificacion.UsuarioId,
            Titulo = notificacion.Titulo,
            Mensaje = notificacion.Mensaje,
            Leido = notificacion.Leido,
            Fecha = notificacion.Fecha
        };
    }

    public async Task MarcarComoLeidaAsync(int id)
    {
        var notificacion = await _notificacionRepository.ObtenerPorIdAsync(id);
        if (notificacion != null)
        {
            notificacion.Leido = true;
            await _notificacionRepository.GuardarCambiosAsync();
        }
    }
}
