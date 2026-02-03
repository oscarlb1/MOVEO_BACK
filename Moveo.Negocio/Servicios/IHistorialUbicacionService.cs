using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IHistorialUbicacionService
{
    Task<IEnumerable<HistorialUbicacionDto>> ObtenerTodosAsync();
    Task<IEnumerable<HistorialUbicacionDto>> ObtenerPorRutaIdAsync(int rutaId);
    Task<HistorialUbicacionDto> CrearAsync(CrearHistorialUbicacionDto dto);
}
