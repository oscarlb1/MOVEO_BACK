using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

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
        return clientes.Select(c => new ClienteDto
        {
            Id = c.Id,
            NombreEmpresa = c.NombreEmpresa,
            Direccion = c.Direccion,
            Latitud = c.Latitud,
            Longitud = c.Longitud,
            Telefono = c.Telefono
        });
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
    {
        var c = await _clienteRepository.ObtenerPorIdAsync(id);
        if (c == null) return null;

        return new ClienteDto
        {
            Id = c.Id,
            NombreEmpresa = c.NombreEmpresa,
            Direccion = c.Direccion,
            Latitud = c.Latitud,
            Longitud = c.Longitud,
            Telefono = c.Telefono
        };
    }

    public async Task<ClienteDto> CrearAsync(CrearClienteDto dto)
    {
        var cliente = new Cliente
        {
            NombreEmpresa = dto.NombreEmpresa,
            Direccion = dto.Direccion,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            Telefono = dto.Telefono
        };

        await _clienteRepository.AgregarAsync(cliente);

        return new ClienteDto
        {
            Id = cliente.Id,
            NombreEmpresa = cliente.NombreEmpresa,
            Direccion = cliente.Direccion,
            Latitud = cliente.Latitud,
            Longitud = cliente.Longitud,
            Telefono = cliente.Telefono
        };
    }
}
