using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IMantenimientoRepository
{
    Task<IEnumerable<Mantenimiento>> ObtenerTodosAsync();
    Task<Mantenimiento?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Mantenimiento>> ObtenerPorVehiculoIdAsync(int vehiculoId);
    Task AgregarAsync(Mantenimiento mantenimiento);
    Task ActualizarAsync(Mantenimiento mantenimiento);
    Task EliminarAsync(Mantenimiento mantenimiento);
    Task GuardarCambiosAsync();
}
