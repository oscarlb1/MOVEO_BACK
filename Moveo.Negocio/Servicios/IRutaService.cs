using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IRutaService
{
    Task<IEnumerable<RutaDto>> ObtenerTodosAsync();
    Task<RutaDto?> ObtenerPorIdAsync(int id);
    Task<RutaDto> CrearAsync(CrearRutaDto dto);
}
