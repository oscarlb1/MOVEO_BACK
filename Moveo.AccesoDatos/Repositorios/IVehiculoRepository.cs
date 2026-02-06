using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IVehiculoRepository
{
    Task<IEnumerable<Vehiculo>> ObtenerTodosAsync();
    Task<Vehiculo?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Vehiculo vehiculo);
    Task ActualizarAsync(Vehiculo vehiculo);
    Task EliminarAsync(int id);
    Task GuardarCambiosAsync();
}
