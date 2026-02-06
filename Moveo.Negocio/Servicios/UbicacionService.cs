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
}
