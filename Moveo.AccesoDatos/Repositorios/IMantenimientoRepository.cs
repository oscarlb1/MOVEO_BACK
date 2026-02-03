using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IMantenimientoRepository
{
    Task<IEnumerable<Mantenimiento>> ObtenerTodosAsync();
    Task<Mantenimiento?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Mantenimiento mantenimiento);
    Task GuardarCambiosAsync();
}
