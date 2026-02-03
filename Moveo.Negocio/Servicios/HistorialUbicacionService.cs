using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class HistorialUbicacionService : IHistorialUbicacionService
{
    private readonly IHistorialUbicacionRepository _historialUbicacionRepository;

    public HistorialUbicacionService(IHistorialUbicacionRepository historialUbicacionRepository)
    {
        _historialUbicacionRepository = historialUbicacionRepository;
    }

    public async Task<IEnumerable<HistorialUbicacionDto>> ObtenerTodosAsync()
    {
        var historiales = await _historialUbicacionRepository.ObtenerTodosAsync();
        return historiales.Select(h => new HistorialUbicacionDto
        {
            Id = h.Id,
            RutaId = h.RutaId,
            Latitud = h.Latitud,
            Longitud = h.Longitud,
            FechaHora = h.FechaHora
        });
    }

    public async Task<IEnumerable<HistorialUbicacionDto>> ObtenerPorRutaIdAsync(int rutaId)
    {
        var historiales = await _historialUbicacionRepository.ObtenerPorRutaIdAsync(rutaId);
        return historiales.Select(h => new HistorialUbicacionDto
        {
            Id = h.Id,
            RutaId = h.RutaId,
            Latitud = h.Latitud,
            Longitud = h.Longitud,
            FechaHora = h.FechaHora
        });
    }

    public async Task<HistorialUbicacionDto> CrearAsync(CrearHistorialUbicacionDto dto)
    {
        var historial = new HistorialUbicacion
        {
            RutaId = dto.RutaId,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            FechaHora = DateTime.UtcNow
        };

        await _historialUbicacionRepository.AgregarAsync(historial);

        return new HistorialUbicacionDto
        {
            Id = historial.Id,
            RutaId = historial.RutaId,
            Latitud = historial.Latitud,
            Longitud = historial.Longitud,
            FechaHora = historial.FechaHora
        };
    }
}
