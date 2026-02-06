using System.Collections.Generic;
using System.Threading.Tasks;
using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IRutaRepository
{
    Task<IEnumerable<Ruta>> ObtenerTodasAsync();
    Task<Ruta?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Ruta>> ObtenerPorConductorIdAsync(int conductorId);
    Task<IEnumerable<Ruta>> ObtenerPorVehiculoIdAsync(int vehiculoId);
    Task AgregarAsync(Ruta ruta);
    Task ActualizarAsync(Ruta ruta);
    Task EliminarAsync(Ruta ruta);
    Task GuardarCambiosAsync();
}
