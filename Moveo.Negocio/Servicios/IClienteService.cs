using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> ObtenerTodosAsync();
    Task<ClienteDto?> ObtenerPorIdAsync(int id);
    Task<ClienteDto> CrearAsync(CrearClienteDto dto);
}
