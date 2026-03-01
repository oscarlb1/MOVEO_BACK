using System.Collections.Generic;
using System.Threading.Tasks;
using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IRutaService
{
    Task<IEnumerable<RutaDto>> ObtenerTodasAsync(string? estado = null, int? conductorId = null, int? vehiculoId = null);
    Task<RutaDetalleDto?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<RutaDto>> ObtenerMisRutasAsync(int conductorId);
    Task<RutaDto> CrearAsync(CrearRutaDto rutaDto);
    Task<bool> ActualizarAsync(int id, ActualizarRutaDto rutaDto);
    Task<bool> ActualizarEstadoAsync(int id, string nuevoEstado);
    Task<bool> EliminarAsync(int id);
    Task<RutaEstadisticasDto> ObtenerEstadisticasAsync();
    Task<OptimizacionIaResponseDto> OptimizarRutaAsync(int id);
}
