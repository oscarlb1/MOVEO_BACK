using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface INotificacionRepository
{
    Task<IEnumerable<Notificacion>> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<Notificacion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Notificacion notificacion);
    Task GuardarCambiosAsync();
}
