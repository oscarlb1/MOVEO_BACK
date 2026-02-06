using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IEstadoSesionRepository
{
    Task<EstadoSesion?> ObtenerPorIdAsync(int id);
    Task<EstadoSesion?> ObtenerActivaPorUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<EstadoSesion>> ObtenerActivasAsync();
    Task<IEnumerable<EstadoSesion>> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task AgregarAsync(EstadoSesion sesion);
    Task ActualizarAsync(EstadoSesion sesion);
    Task GuardarCambiosAsync();
}
