using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IUbicacionRepository
{
    Task RegistrarPuntoAsync(UbicacionHistorial punto);
    Task<IEnumerable<UbicacionHistorial>> ObtenerPorRutaAsync(int rutaId);
}
