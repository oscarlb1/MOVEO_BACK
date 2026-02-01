using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IVehiculoRepository
{
    Task<IEnumerable<Vehiculo>> ObtenerTodosAsync();
    Task<Vehiculo?> ObtenerPorIdAsync(int id);
    Task ActualizarAsync(Vehiculo vehiculo);
    Task GuardarCambiosAsync();
}
