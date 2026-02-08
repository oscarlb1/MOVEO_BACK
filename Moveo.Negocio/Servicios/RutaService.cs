using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class RutaService : IRutaService
{
    private readonly IRutaRepository _rutaRepository;

    public RutaService(IRutaRepository rutaRepository)
    {
        _rutaRepository = rutaRepository;
    }

    public async Task<IEnumerable<RutaDto>> ObtenerTodasAsync()
    {
        var rutas = await _rutaRepository.ObtenerTodasAsync();
        return rutas.Select(MapToDto);
    }

    public async Task<RutaDto?> ObtenerPorIdAsync(int id)
    {
        var ruta = await _rutaRepository.ObtenerPorIdAsync(id);
        if (ruta == null) return null;
        return MapToDto(ruta);
    }

    public async Task<RutaDto> CrearAsync(CrearRutaDto rutaDto)
    {
        var ruta = new Ruta
        {
            Fecha = rutaDto.Fecha,
            ConductorId = rutaDto.ConductorId,
            VehiculoId = rutaDto.VehiculoId,
            Estado = rutaDto.Estado ?? "Planificada",
            DistanciaTotalEstimada = rutaDto.DistanciaTotalEstimada
        };

        if (ruta.Fecha.Kind != DateTimeKind.Utc)
        {
             ruta.Fecha = DateTime.SpecifyKind(ruta.Fecha, DateTimeKind.Utc);
        }

        await _rutaRepository.AgregarAsync(ruta);
        return MapToDto(ruta);
    }

    public async Task<bool> ActualizarAsync(int id, ActualizarRutaDto rutaDto)
    {
        var ruta = await _rutaRepository.ObtenerPorIdAsync(id);
        if (ruta == null) return false;

        ruta.Fecha = rutaDto.Fecha.Kind == DateTimeKind.Utc ? rutaDto.Fecha : DateTime.SpecifyKind(rutaDto.Fecha, DateTimeKind.Utc);
        ruta.ConductorId = rutaDto.ConductorId;
        ruta.VehiculoId = rutaDto.VehiculoId;
        if (!string.IsNullOrEmpty(rutaDto.Estado))
        {
            ruta.Estado = rutaDto.Estado;
        }
        ruta.DistanciaTotalEstimada = rutaDto.DistanciaTotalEstimada;

        await _rutaRepository.ActualizarAsync(ruta);
        await _rutaRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var ruta = await _rutaRepository.ObtenerPorIdAsync(id);
        if (ruta == null) return false;

        await _rutaRepository.EliminarAsync(ruta);
        return true;
    }

    private static RutaDto MapToDto(Ruta r)
    {
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
}
