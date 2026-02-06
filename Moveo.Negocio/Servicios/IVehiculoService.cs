using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IVehiculoService
{
    Task<IEnumerable<VehiculoDto>> ObtenerTodosLosVehiculosAsync();
    Task<VehiculoDto?> ObtenerVehiculoPorIdAsync(int id);
    Task<VehiculoDto> CrearVehiculoAsync(CreateVehiculoDto createVehiculoDto);
    Task<VehiculoDto?> ActualizarVehiculoAsync(int id, UpdateVehiculoDto updateVehiculoDto);
    Task<bool> EliminarVehiculoAsync(int id);
}
