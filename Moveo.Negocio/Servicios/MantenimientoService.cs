using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class MantenimientoService : IMantenimientoService
{
    private readonly IMantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(IMantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }

    public async Task<IEnumerable<MantenimientoDto>> ObtenerTodosAsync()
    {
        var mantenimientos = await _mantenimientoRepository.ObtenerTodosAsync();
        return mantenimientos.Select(m => new MantenimientoDto
        {
            Id = m.Id,
            VehiculoId = m.VehiculoId,
            FechaServicio = m.FechaServicio,
            TipoMantenimiento = m.TipoMantenimiento,
            KilometrajeServicio = m.KilometrajeServicio,
            Coste = m.Coste
        });
    }

    public async Task<MantenimientoDto?> ObtenerPorIdAsync(int id)
    {
        var m = await _mantenimientoRepository.ObtenerPorIdAsync(id);
        if (m == null) return null;

        return new MantenimientoDto
        {
            Id = m.Id,
            VehiculoId = m.VehiculoId,
            FechaServicio = m.FechaServicio,
            TipoMantenimiento = m.TipoMantenimiento,
            KilometrajeServicio = m.KilometrajeServicio,
            Coste = m.Coste
        };
    }

    public async Task<MantenimientoDto> CrearAsync(CrearMantenimientoDto dto)
    {
        var mantenimiento = new Mantenimiento
        {
            VehiculoId = dto.VehiculoId,
            FechaServicio = dto.FechaServicio,
            TipoMantenimiento = dto.TipoMantenimiento,
            KilometrajeServicio = dto.KilometrajeServicio,
            Coste = dto.Coste
        };

        await _mantenimientoRepository.AgregarAsync(mantenimiento);

        return new MantenimientoDto
        {
            Id = mantenimiento.Id,
            VehiculoId = mantenimiento.VehiculoId,
            FechaServicio = mantenimiento.FechaServicio,
            TipoMantenimiento = mantenimiento.TipoMantenimiento,
            KilometrajeServicio = mantenimiento.KilometrajeServicio,
            Coste = mantenimiento.Coste
        };
    }
}
