using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface INotificacionService
{
    Task<IEnumerable<NotificacionDto>> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<NotificacionDto?> ObtenerPorIdAsync(int id);
    Task<NotificacionDto> CrearAsync(CrearNotificacionDto dto);
    Task MarcarComoLeidaAsync(int id);
}
