using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IMantenimientoService
{
    Task<IEnumerable<MantenimientoDto>> ObtenerTodosAsync();
    Task<MantenimientoDto?> ObtenerPorIdAsync(int id);
    Task<MantenimientoDto> CrearAsync(CrearMantenimientoDto dto);
}
