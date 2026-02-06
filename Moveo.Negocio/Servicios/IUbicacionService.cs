using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IUbicacionService
{
    Task RegistrarUbicacionAsync(RegistroUbicacionDto dto);
    Task<IEnumerable<UbicacionDto>> ObtenerHistorialPorRutaAsync(int rutaId);
    Task<UbicacionDto?> ObtenerUltimaUbicacionAsync(int rutaId);
    Task<bool> ActualizarUbicacionAsync(int id, RegistroUbicacionDto dto);
    Task<bool> EliminarUbicacionAsync(int id);
    Task EliminarHistorialRutaAsync(int rutaId);
}
