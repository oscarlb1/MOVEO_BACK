using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class VehiculoService : IVehiculoService
{
    private readonly IVehiculoRepository _vehiculoRepository;

    public VehiculoService(IVehiculoRepository vehiculoRepository)
    {
        _vehiculoRepository = vehiculoRepository;
    }

    public async Task<IEnumerable<VehiculoDto>> ObtenerTodosLosVehiculosAsync()
    {
        var vehiculos = await _vehiculoRepository.ObtenerTodosAsync();
        return vehiculos.Select(v => MapToDto(v));
    }

    public async Task<VehiculoDto?> ObtenerVehiculoPorIdAsync(int id)
    {
        var vehiculo = await _vehiculoRepository.ObtenerPorIdAsync(id);
        if (vehiculo == null) return null;
        return MapToDto(vehiculo);
    }

    public async Task<VehiculoDto?> ActualizarVehiculoAsync(int id, UpdateVehiculoDto updateVehiculoDto)
    {
        var vehiculo = await _vehiculoRepository.ObtenerPorIdAsync(id);
        if (vehiculo == null) return null;

        // Partial update: only update fields that are not null in the DTO
        if (updateVehiculoDto.Matricula != null) vehiculo.Matricula = updateVehiculoDto.Matricula;
        if (updateVehiculoDto.MarcaModelo != null) vehiculo.MarcaModelo = updateVehiculoDto.MarcaModelo;
        if (updateVehiculoDto.Estado != null) vehiculo.Estado = updateVehiculoDto.Estado;
        if (updateVehiculoDto.CapacidadCarga.HasValue) vehiculo.CapacidadCarga = updateVehiculoDto.CapacidadCarga.Value;
        if (updateVehiculoDto.ConsumoMedio.HasValue) vehiculo.ConsumoMedio = updateVehiculoDto.ConsumoMedio.Value;
        if (updateVehiculoDto.KilometrajeActual.HasValue) vehiculo.KilometrajeActual = updateVehiculoDto.KilometrajeActual.Value;
        if (updateVehiculoDto.FechaUltimaRevision.HasValue)
            vehiculo.FechaUltimaRevision = DateTime.SpecifyKind(updateVehiculoDto.FechaUltimaRevision.Value, DateTimeKind.Utc);

        await _vehiculoRepository.ActualizarAsync(vehiculo);
        await _vehiculoRepository.GuardarCambiosAsync();

        return MapToDto(vehiculo);
    }

    private static VehiculoDto MapToDto(Vehiculo v)
    {
        return new VehiculoDto
        {
            Id = v.Id,
            Matricula = v.Matricula,
            MarcaModelo = v.MarcaModelo,
            Estado = v.Estado,
            CapacidadCarga = v.CapacidadCarga,
            ConsumoMedio = v.ConsumoMedio,
            KilometrajeActual = v.KilometrajeActual,
            FechaUltimaRevision = v.FechaUltimaRevision
        };
    }
}
