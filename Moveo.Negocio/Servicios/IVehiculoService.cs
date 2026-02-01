using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IVehiculoService
{
    Task<IEnumerable<VehiculoDto>> ObtenerTodosLosVehiculosAsync();
    Task<VehiculoDto?> ObtenerVehiculoPorIdAsync(int id);
    Task<VehiculoDto?> ActualizarVehiculoAsync(int id, UpdateVehiculoDto updateVehiculoDto);
}
