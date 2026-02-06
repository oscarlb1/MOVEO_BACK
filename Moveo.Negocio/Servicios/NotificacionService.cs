using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public NotificacionService(INotificacionRepository notificacionRepository, IUsuarioRepository usuarioRepository)
    {
        _notificacionRepository = notificacionRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<NotificacionDto>> ObtenerMisNotificacionesAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        return notificaciones.Select(n => MapToDto(n));
    }

    public async Task<IEnumerable<NotificacionDto>> ObtenerTodasAdminAsync()
    {
        var notificaciones = await _notificacionRepository.ObtenerTodasAsync();
        return notificaciones.Select(n => MapToDto(n));
    }

    public async Task EnviarNotificacionAsync(CrearNotificacionDto dto)
    {
        var notificacion = new Notificacion
        {
            UsuarioId = dto.UsuarioId ?? 0,
            Titulo = dto.Titulo,
            Mensaje = dto.Mensaje,
            Fecha = DateTime.UtcNow,
            Leido = false
        };

        await _notificacionRepository.AgregarAsync(notificacion);
    }

    public async Task EnviarABroadcastAsync(CrearNotificacionDto dto)
    {
        var usuarios = await _usuarioRepository.ObtenerTodosAsync();
        var notificaciones = usuarios.Select(u => new Notificacion
        {
            UsuarioId = u.Id,
            Titulo = dto.Titulo,
            Mensaje = dto.Mensaje,
            Fecha = DateTime.UtcNow,
            Leido = false
        });

        await _notificacionRepository.AgregarVariasAsync(notificaciones);
    }

    public async Task<bool> MarcarComoLeidaAsync(int id)
    {
        var notificacion = await _notificacionRepository.ObtenerPorIdAsync(id);
        if (notificacion == null) return false;

        notificacion.Leido = true;
        await _notificacionRepository.ActualizarAsync(notificacion);
        return true;
    }

    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        await _notificacionRepository.MarcarTodasComoLeidasAsync(usuarioId);
    }

    public async Task<int> ObtenerConteoNoLeidasAsync(int usuarioId)
    {
        return await _notificacionRepository.ObtenerConteoNoLeidasAsync(usuarioId);
    }

    public async Task<bool> EliminarNotificacionAsync(int id)
    {
        var notificacion = await _notificacionRepository.ObtenerPorIdAsync(id);
        if (notificacion == null) return false;

        await _notificacionRepository.EliminarAsync(id);
        return true;
    }

    private static NotificacionDto MapToDto(Notificacion n)
    {
        return new NotificacionDto
        {
            Id = n.Id,
            UsuarioId = n.UsuarioId,
            NombreUsuario = n.Usuario?.Nombre ?? "Global",
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            Leido = n.Leido,
            Fecha = n.Fecha
        };
    }
}
