using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IEstadisticaUsuarioService
{
    Task<EstadisticaUsuarioDto?> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<EstadisticaUsuarioDto> CrearOActualizarAsync(int usuarioId, ActualizarEstadisticaUsuarioDto dto);
}
