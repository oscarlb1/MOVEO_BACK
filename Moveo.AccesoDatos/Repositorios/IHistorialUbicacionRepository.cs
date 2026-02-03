using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IHistorialUbicacionRepository
{
    Task<IEnumerable<HistorialUbicacion>> ObtenerTodosAsync();
    Task<IEnumerable<HistorialUbicacion>> ObtenerPorRutaIdAsync(int rutaId);
    Task AgregarAsync(HistorialUbicacion historial);
    Task GuardarCambiosAsync();
}
