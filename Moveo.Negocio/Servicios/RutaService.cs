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

    public async Task<IEnumerable<RutaDto>> ObtenerTodasAsync(string? estado = null, int? conductorId = null, int? vehiculoId = null)
    {
        var rutas = await _rutaRepository.ObtenerTodasAsync(estado, conductorId, vehiculoId);
        return rutas.Select(MapToDto);
    }

    public async Task<RutaDetalleDto?> ObtenerPorIdAsync(int id)
    {
        var ruta = await _rutaRepository.ObtenerPorIdAsync(id);
        if (ruta == null) return null;

        var dto = new RutaDetalleDto
        {
            Id = ruta.Id,
            Fecha = ruta.Fecha,
            ConductorId = ruta.ConductorId,
            NombreConductor = ruta.Conductor?.Nombre ?? "N/A",
            VehiculoId = ruta.VehiculoId,
            MatriculaVehiculo = ruta.Vehiculo?.Matricula ?? "N/A",
            Estado = ruta.Estado,
            DistanciaTotalEstimada = ruta.DistanciaTotalEstimada,
            Entregas = ruta.Entregas.Select(e => new EntregaDto(
                e.Id,
                e.RutaId,
                e.ClienteId,
                e.Cliente != null ? new ClienteDto(
                    e.Cliente.Id,
                    e.Cliente.NombreEmpresa,
                    e.Cliente.Direccion,
                    e.Cliente.Telefono,
                    e.Cliente.Latitud,
                    e.Cliente.Longitud,
                    e.Cliente.CreatedAt,
                    e.Cliente.UpdatedAt
                ) : null,
                e.OrdenParada,
                e.Estado,
                e.HoraEntregaReal,
                e.FotoUrl,
                e.FirmaDigitalUrl,
                e.Notas,
                e.CodigoQr,
                e.CreatedAt,
                e.UpdatedAt
            )).ToList()
        };

        return dto;
    }

    public async Task<IEnumerable<RutaDto>> ObtenerMisRutasAsync(int conductorId)
    {
        var rutas = await _rutaRepository.ObtenerPorConductorIdAsync(conductorId);
        return rutas.Select(MapToDto);
    }

    public async Task<RutaDto> CrearAsync(CrearRutaDto rutaDto)
    {
        var ruta = new Ruta
        {
            Fecha = rutaDto.Fecha,
            ConductorId = rutaDto.ConductorId,
            VehiculoId = rutaDto.VehiculoId,
            Estado = rutaDto.Estado ?? "PENDIENTE",
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
        return true;
    }

    public async Task<bool> ActualizarEstadoAsync(int id, string nuevoEstado)
    {
        var ruta = await _rutaRepository.ObtenerPorIdAsync(id);
        if (ruta == null) return false;

        ruta.Estado = nuevoEstado;
        await _rutaRepository.ActualizarAsync(ruta);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var ruta = await _rutaRepository.ObtenerPorIdAsync(id);
        if (ruta == null) return false;

        await _rutaRepository.EliminarAsync(ruta);
        return true;
    }

    public async Task<RutaEstadisticasDto> ObtenerEstadisticasAsync()
    {
        return new RutaEstadisticasDto
        {
            TotalRutas = (await _rutaRepository.ObtenerTodasAsync()).Count(),
            Planificadas = await _rutaRepository.ObtenerConteoPorEstadoAsync("PENDIENTE"),
            EnProgreso = await _rutaRepository.ObtenerConteoPorEstadoAsync("EN_PROGRESO"),
            Completadas = await _rutaRepository.ObtenerConteoPorEstadoAsync("COMPLETADA"),
            Canceladas = await _rutaRepository.ObtenerConteoPorEstadoAsync("CANCELADA")
        };
    }

    private static RutaDto MapToDto(Ruta r)
    {
        return new RutaDto
        {
            Id = r.Id,
            Fecha = r.Fecha,
            ConductorId = r.ConductorId,
            NombreConductor = r.Conductor?.Nombre ?? "N/A",
            VehiculoId = r.VehiculoId,
            MatriculaVehiculo = r.Vehiculo?.Matricula ?? "N/A",
            Estado = r.Estado,
            DistanciaTotalEstimada = r.DistanciaTotalEstimada
        };
    }
}
