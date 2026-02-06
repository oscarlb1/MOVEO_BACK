using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IUbicacionRepository
{
    Task RegistrarPuntoAsync(UbicacionHistorial punto);
    Task<IEnumerable<UbicacionHistorial>> ObtenerPorRutaAsync(int rutaId);
    Task<UbicacionHistorial?> ObtenerPorIdAsync(int id);
    Task ActualizarPuntoAsync(UbicacionHistorial punto);
    Task EliminarPuntoAsync(int id);
    Task EliminarPorRutaAsync(int rutaId);
}
