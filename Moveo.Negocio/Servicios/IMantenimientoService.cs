using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IMantenimientoService
{
    Task<IEnumerable<MantenimientoDto>> ObtenerTodosLosMantenimientosAsync();
    Task<MantenimientoDto?> ObtenerMantenimientoPorIdAsync(int id);
    Task<IEnumerable<MantenimientoDto>> ObtenerMantenimientosPorVehiculoIdAsync(int vehiculoId);
    Task<MantenimientoDto> CrearMantenimientoAsync(CrearMantenimientoDto dto);
    Task<bool> ActualizarMantenimientoAsync(int id, ActualizarMantenimientoDto dto);
    Task<bool> EliminarMantenimientoAsync(int id);
}
