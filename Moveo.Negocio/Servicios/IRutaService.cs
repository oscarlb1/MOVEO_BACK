using System.Collections.Generic;
using System.Threading.Tasks;
using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IRutaService
{
    Task<IEnumerable<RutaDto>> ObtenerTodasAsync();
    Task<RutaDto?> ObtenerPorIdAsync(int id);
    Task<RutaDto> CrearAsync(CrearRutaDto rutaDto);
    Task<bool> ActualizarAsync(int id, ActualizarRutaDto rutaDto);
    Task<bool> EliminarAsync(int id);
}
