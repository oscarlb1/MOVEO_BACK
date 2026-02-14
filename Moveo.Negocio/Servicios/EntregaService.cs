using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class EntregaService : IEntregaService
{
    private readonly IEntregaRepository _entregaRepository;
    private readonly IClienteRepository _clienteRepository;

    public EntregaService(IEntregaRepository entregaRepository, IClienteRepository clienteRepository)
    {
        _entregaRepository = entregaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<EntregaDto>> ObtenerTodasAsync(int? rutaId, int? clienteId, string? estado, DateTime? fecha)
    {
        var entregas = await _entregaRepository.ObtenerPorFiltrosAsync(rutaId, clienteId, estado, fecha);
        return entregas.Select(MapToDto);
    }

    public async Task<EntregaDto?> ObtenerPorIdAsync(int id)
    {
        var entrega = await _entregaRepository.ObtenerPorIdAsync(id);
        return entrega == null ? null : MapToDto(entrega);
    }

    public async Task<IEnumerable<EntregaDto>> ObtenerPorRutaAsync(int rutaId)
    {
        var entregas = await _entregaRepository.ObtenerPorRutaAsync(rutaId);
        return entregas.Select(MapToDto);
    }

    public async Task<EntregaDto> CrearAsync(CrearEntregaDto crearEntregaDto)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(crearEntregaDto.ClienteId);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {crearEntregaDto.ClienteId} no encontrado.");
        }

        var entrega = new Entrega
        {
            RutaId = crearEntregaDto.RutaId,
            ClienteId = crearEntregaDto.ClienteId,
            OrdenParada = crearEntregaDto.OrdenParada,
            Notas = crearEntregaDto.Notas,
            CodigoQr = crearEntregaDto.CodigoQr,
            Estado = "Pendiente",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _entregaRepository.AgregarAsync(entrega);
        
        // Cargar el cliente para el DTO de respuesta
        entrega.Cliente = cliente;
        
        return MapToDto(entrega);
    }

    public async Task ActualizarEstadoAsync(int id, ActualizarEstadoEntregaDto dto)
    {
        var entrega = await _entregaRepository.ObtenerPorIdAsync(id);
        if (entrega == null)
        {
            throw new KeyNotFoundException($"Entrega con ID {id} no encontrada.");
        }

        entrega.Estado = dto.Estado;
        if (!string.IsNullOrEmpty(dto.FotoUrl)) entrega.FotoUrl = dto.FotoUrl;
        if (!string.IsNullOrEmpty(dto.FirmaDigitalUrl)) entrega.FirmaDigitalUrl = dto.FirmaDigitalUrl;
        if (!string.IsNullOrEmpty(dto.Notas)) entrega.Notas = dto.Notas;

        if (dto.Estado.Equals("Completado", StringComparison.OrdinalIgnoreCase) || 
            dto.Estado.Equals("Entregado", StringComparison.OrdinalIgnoreCase))
        {
            entrega.HoraEntregaReal = DateTime.UtcNow;
        }

        entrega.UpdatedAt = DateTime.UtcNow;

        await _entregaRepository.ActualizarAsync(entrega);
    }

    public async Task EliminarAsync(int id)
    {
        await _entregaRepository.EliminarAsync(id);
    }

    public async Task<EntregaEstadisticasDto> ObtenerEstadisticasDelDiaAsync()
    {
        var entregasHoy = await _entregaRepository.ObtenerDelDiaAsync(DateTime.UtcNow);
        var total = entregasHoy.Count();
        var pendientes = entregasHoy.Count(e => e.Estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase) || e.Estado.Equals("EnProgreso", StringComparison.OrdinalIgnoreCase));
        var completadas = entregasHoy.Count(e => e.Estado.Equals("Completado", StringComparison.OrdinalIgnoreCase) || 
                                                e.Estado.Equals("Entregado", StringComparison.OrdinalIgnoreCase));
        var fallidas = entregasHoy.Count(e => e.Estado.Equals("Fallido", StringComparison.OrdinalIgnoreCase) || 
                                             e.Estado.Equals("Cancelado", StringComparison.OrdinalIgnoreCase));

        return new EntregaEstadisticasDto(total, pendientes, completadas, fallidas);
    }

    private static EntregaDto MapToDto(Entrega e)
    {
        return new EntregaDto(
            e.Id,
            e.RutaId,
            e.ClienteId,
            e.Cliente == null ? null : new ClienteDto(
                e.Cliente.Id,
                e.Cliente.NombreEmpresa,
                e.Cliente.Direccion,
                e.Cliente.Telefono,
                e.Cliente.Latitud,
                e.Cliente.Longitud,
                e.Cliente.CreatedAt,
                e.Cliente.UpdatedAt
            ),
            e.OrdenParada,
            e.Estado,
            e.HoraEntregaReal,
            e.FotoUrl,
            e.FirmaDigitalUrl,
            e.Notas,
            e.CodigoQr,
            e.CreatedAt,
            e.UpdatedAt
        );
    }
}
