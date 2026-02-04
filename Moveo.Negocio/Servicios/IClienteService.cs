using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> ObtenerTodosAsync();
    Task<ClienteDto?> ObtenerPorIdAsync(int id);
    Task<ClienteDto> CrearAsync(CrearClienteDto crearClienteDto);
    Task ActualizarAsync(int id, CrearClienteDto clienteDto);
    Task EliminarAsync(int id);
}
