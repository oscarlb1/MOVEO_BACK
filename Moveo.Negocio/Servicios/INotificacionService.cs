using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface INotificacionService
{
    Task<IEnumerable<NotificacionDto>> ObtenerMisNotificacionesAsync(int usuarioId);
    Task<IEnumerable<NotificacionDto>> ObtenerTodasAdminAsync();
    Task EnviarNotificacionAsync(CrearNotificacionDto dto);
    Task EnviarABroadcastAsync(CrearNotificacionDto dto);
    Task<bool> MarcarComoLeidaAsync(int id);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
    Task<int> ObtenerConteoNoLeidasAsync(int usuarioId);
    Task<bool> EliminarNotificacionAsync(int id);
}
