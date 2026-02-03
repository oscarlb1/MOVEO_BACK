using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class RutaService : IRutaService
{
    private readonly IRutaRepository _rutaRepository;

    public RutaService(IRutaRepository rutaRepository)
    {
        _rutaRepository = rutaRepository;
    }

    public async Task<IEnumerable<RutaDto>> ObtenerTodosAsync()
    {
        var rutas = await _rutaRepository.ObtenerTodosAsync();
        return rutas.Select(r => new RutaDto
        {
            Id = r.Id,
            Fecha = r.Fecha,
            ConductorId = r.ConductorId,
            VehiculoId = r.VehiculoId,
            Estado = r.Estado,
            DistanciaTotalEstimada = r.DistanciaTotalEstimada
        });
    }

    public async Task<RutaDto?> ObtenerPorIdAsync(int id)
    {
        var r = await _rutaRepository.ObtenerPorIdAsync(id);
        if (r == null) return null;

        return new RutaDto
        {
            Id = r.Id,
            Fecha = r.Fecha,
            ConductorId = r.ConductorId,
            VehiculoId = r.VehiculoId,
            Estado = r.Estado,
            DistanciaTotalEstimada = r.DistanciaTotalEstimada
        };
    }

    public async Task<RutaDto> CrearAsync(CrearRutaDto dto)
    {
        var ruta = new Ruta
        {
            Fecha = dto.Fecha,
            ConductorId = dto.ConductorId,
            VehiculoId = dto.VehiculoId,
            Estado = "Planificada",
            DistanciaTotalEstimada = dto.DistanciaTotalEstimada
        };

        await _rutaRepository.AgregarAsync(ruta);

        return new RutaDto
        {
            Id = ruta.Id,
            Fecha = ruta.Fecha,
            ConductorId = ruta.ConductorId,
            VehiculoId = ruta.VehiculoId,
            Estado = ruta.Estado,
            DistanciaTotalEstimada = ruta.DistanciaTotalEstimada
        };
    }
}
