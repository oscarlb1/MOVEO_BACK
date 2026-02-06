using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class UbicacionService : IUbicacionService
{
    private readonly IUbicacionRepository _ubicacionRepository;

    public UbicacionService(IUbicacionRepository ubicacionRepository)
    {
        _ubicacionRepository = ubicacionRepository;
    }

    public async Task RegistrarUbicacionAsync(RegistroUbicacionDto dto)
    {
        var punto = new UbicacionHistorial
        {
            RutaId = dto.RutaId,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            FechaHora = DateTime.UtcNow
        };

        await _ubicacionRepository.RegistrarPuntoAsync(punto);
    }

    public async Task<IEnumerable<UbicacionDto>> ObtenerHistorialPorRutaAsync(int rutaId)
    {
        var puntos = await _ubicacionRepository.ObtenerPorRutaAsync(rutaId);
        return puntos.Select(p => new UbicacionDto
        {
            Id = p.Id,
            RutaId = p.RutaId,
            Latitud = p.Latitud,
            Longitud = p.Longitud,
            FechaHora = p.FechaHora
        });
    }

    public async Task<UbicacionDto?> ObtenerUltimaUbicacionAsync(int rutaId)
    {
        var puntos = await _ubicacionRepository.ObtenerPorRutaAsync(rutaId);
        var ultimo = puntos.LastOrDefault();
        if (ultimo == null) return null;

        return new UbicacionDto
        {
            Id = ultimo.Id,
            RutaId = ultimo.RutaId,
            Latitud = ultimo.Latitud,
            Longitud = ultimo.Longitud,
            FechaHora = ultimo.FechaHora
        };
    }

    public async Task<bool> ActualizarUbicacionAsync(int id, RegistroUbicacionDto dto)
    {
        var punto = await _ubicacionRepository.ObtenerPorIdAsync(id);
        if (punto == null) return false;

        punto.Latitud = dto.Latitud;
        punto.Longitud = dto.Longitud;

        await _ubicacionRepository.ActualizarPuntoAsync(punto);
        return true;
    }

    public async Task<bool> EliminarUbicacionAsync(int id)
    {
        var punto = await _ubicacionRepository.ObtenerPorIdAsync(id);
        if (punto == null) return false;

        await _ubicacionRepository.EliminarPuntoAsync(id);
        return true;
    }

    public async Task EliminarHistorialRutaAsync(int rutaId)
    {
        await _ubicacionRepository.EliminarPorRutaAsync(rutaId);
    }
}
