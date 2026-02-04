using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ClienteDto>> ObtenerTodosAsync()
    {
        var clientes = await _clienteRepository.ObtenerTodosAsync();
        return clientes.Select(c => new ClienteDto(
            c.Id,
            c.NombreEmpresa,
            c.Direccion,
            c.Telefono,
            c.Latitud,
            c.Longitud,
            c.CreatedAt,
            c.UpdatedAt
        ));
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
        if (cliente == null) return null;

        return new ClienteDto(
            cliente.Id,
            cliente.NombreEmpresa,
            cliente.Direccion,
            cliente.Telefono,
            cliente.Latitud,
            cliente.Longitud,
            cliente.CreatedAt,
            cliente.UpdatedAt
        );
    }

    public async Task<ClienteDto> CrearAsync(CrearClienteDto crearClienteDto)
    {
        var cliente = new Cliente
        {
            NombreEmpresa = crearClienteDto.NombreEmpresa,
            Direccion = crearClienteDto.Direccion,
            Telefono = crearClienteDto.Telefono,
            Latitud = crearClienteDto.Latitud,
            Longitud = crearClienteDto.Longitud,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _clienteRepository.AgregarAsync(cliente);

        return new ClienteDto(
            cliente.Id,
            cliente.NombreEmpresa,
            cliente.Direccion,
            cliente.Telefono,
            cliente.Latitud,
            cliente.Longitud,
            cliente.CreatedAt,
            cliente.UpdatedAt
        );
    }

    public async Task ActualizarAsync(int id, CrearClienteDto clienteDto)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");
        }

        cliente.NombreEmpresa = clienteDto.NombreEmpresa;
        cliente.Direccion = clienteDto.Direccion;
        cliente.Telefono = clienteDto.Telefono;
        cliente.Latitud = clienteDto.Latitud;
        cliente.Longitud = clienteDto.Longitud;
        cliente.UpdatedAt = DateTime.UtcNow;

        await _clienteRepository.ActualizarAsync(cliente);
    }

    public async Task EliminarAsync(int id)
    {
        await _clienteRepository.EliminarAsync(id);
    }
}
