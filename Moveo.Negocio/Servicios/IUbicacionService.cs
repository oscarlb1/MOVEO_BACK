using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IUbicacionService
{
    Task RegistrarUbicacionAsync(RegistroUbicacionDto dto);
    Task<IEnumerable<UbicacionDto>> ObtenerHistorialPorRutaAsync(int rutaId);
}
