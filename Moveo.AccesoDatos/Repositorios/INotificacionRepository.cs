using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface INotificacionRepository
{
    Task<IEnumerable<Notificacion>> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<Notificacion>> ObtenerTodasAsync();
    Task<Notificacion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Notificacion notificacion);
    Task AgregarVariasAsync(IEnumerable<Notificacion> notificaciones);
    Task ActualizarAsync(Notificacion notificacion);
    Task EliminarAsync(int id);
    Task MarcarTodasComoLeidasAsync(int usuarioId);
    Task<int> ObtenerConteoNoLeidasAsync(int usuarioId);
}
