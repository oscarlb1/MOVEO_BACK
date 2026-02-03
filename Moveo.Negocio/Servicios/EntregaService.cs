using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class EntregaService : IEntregaService
{
    private readonly IEntregaRepository _entregaRepository;

    public EntregaService(IEntregaRepository entregaRepository)
    {
        _entregaRepository = entregaRepository;
    }

    public async Task<IEnumerable<EntregaDto>> ObtenerTodosAsync()
    {
        var entregas = await _entregaRepository.ObtenerTodosAsync();
        return entregas.Select(e => new EntregaDto
        {
            Id = e.Id,
            RutaId = e.RutaId,
            ClienteId = e.ClienteId,
            OrdenParada = e.OrdenParada,
            Estado = e.Estado,
            CodigoQR = e.CodigoQR,
            FirmaDigitalUrl = e.FirmaDigitalUrl,
            FotoUrl = e.FotoUrl,
            Notas = e.Notas,
            HoraEntregaReal = e.HoraEntregaReal
        });
    }

    public async Task<EntregaDto?> ObtenerPorIdAsync(int id)
    {
        var e = await _entregaRepository.ObtenerPorIdAsync(id);
        if (e == null) return null;

        return new EntregaDto
        {
            Id = e.Id,
            RutaId = e.RutaId,
            ClienteId = e.ClienteId,
            OrdenParada = e.OrdenParada,
            Estado = e.Estado,
            CodigoQR = e.CodigoQR,
            FirmaDigitalUrl = e.FirmaDigitalUrl,
            FotoUrl = e.FotoUrl,
            Notas = e.Notas,
            HoraEntregaReal = e.HoraEntregaReal
        };
    }

    public async Task<EntregaDto> CrearAsync(CrearEntregaDto dto)
    {
        var entrega = new Entrega
        {
            RutaId = dto.RutaId,
            ClienteId = dto.ClienteId,
            OrdenParada = dto.OrdenParada,
            Estado = "Pendiente",
            Notas = dto.Notas
        };

        await _entregaRepository.AgregarAsync(entrega);

        return new EntregaDto
        {
            Id = entrega.Id,
            RutaId = entrega.RutaId,
            ClienteId = entrega.ClienteId,
            OrdenParada = entrega.OrdenParada,
            Estado = entrega.Estado,
            Notas = entrega.Notas
        };
    }
}
