using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IEntregaService
{
    Task<IEnumerable<EntregaDto>> ObtenerTodosAsync();
    Task<EntregaDto?> ObtenerPorIdAsync(int id);
    Task<EntregaDto> CrearAsync(CrearEntregaDto dto);
}
