using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IEntregaService
{
    Task<IEnumerable<EntregaDto>> ObtenerTodasAsync(int? rutaId, int? clienteId, string? estado, DateTime? fecha);
    Task<EntregaDto?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<EntregaDto>> ObtenerPorRutaAsync(int rutaId);
    Task<EntregaDto> CrearAsync(CrearEntregaDto crearEntregaDto);
    Task<EntregaDto?> ActualizarAsync(int id, ActualizarEntregaDto actualizarDto);
    Task ActualizarEstadoAsync(int id, ActualizarEstadoEntregaDto actualizarEstadoDto);
    Task EliminarAsync(int id);
    Task<EntregaEstadisticasDto> ObtenerEstadisticasDelDiaAsync();
}
