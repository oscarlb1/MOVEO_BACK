using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class MantenimientoService : IMantenimientoService
{
    private readonly IMantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(IMantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }

    public async Task<IEnumerable<MantenimientoDto>> ObtenerTodosLosMantenimientosAsync()
    {
        var mantenimientos = await _mantenimientoRepository.ObtenerTodosAsync();
        return mantenimientos.Select(MapToDto);
    }

    public async Task<MantenimientoDto?> ObtenerMantenimientoPorIdAsync(int id)
    {
        var mantenimiento = await _mantenimientoRepository.ObtenerPorIdAsync(id);
        if (mantenimiento == null) return null;
        return MapToDto(mantenimiento);
    }

    public async Task<IEnumerable<MantenimientoDto>> ObtenerMantenimientosPorVehiculoIdAsync(int vehiculoId)
    {
        var mantenimientos = await _mantenimientoRepository.ObtenerPorVehiculoIdAsync(vehiculoId);
        return mantenimientos.Select(MapToDto);
    }

    public async Task<MantenimientoDto> CrearMantenimientoAsync(CrearMantenimientoDto dto)
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
        
        // Return DTO with the generated ID
        return MapToDto(mantenimiento);
    }

    public async Task<bool> ActualizarMantenimientoAsync(int id, ActualizarMantenimientoDto dto)
    {
        var mantenimiento = await _mantenimientoRepository.ObtenerPorIdAsync(id);
        if (mantenimiento == null) return false;

        mantenimiento.FechaServicio = dto.FechaServicio;
        mantenimiento.TipoMantenimiento = dto.TipoMantenimiento;
        mantenimiento.KilometrajeServicio = dto.KilometrajeServicio;
        mantenimiento.Coste = dto.Coste;

        await _mantenimientoRepository.ActualizarAsync(mantenimiento);
        return true;
    }

    public async Task<bool> EliminarMantenimientoAsync(int id)
    {
        var mantenimiento = await _mantenimientoRepository.ObtenerPorIdAsync(id);
        if (mantenimiento == null) return false;

        await _mantenimientoRepository.EliminarAsync(mantenimiento);
        return true;
    }

    private static MantenimientoDto MapToDto(Mantenimiento m)
    {
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
}
