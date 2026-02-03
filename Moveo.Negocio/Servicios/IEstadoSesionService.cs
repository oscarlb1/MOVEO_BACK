using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IEstadoSesionService
{
    Task<EstadoSesionDto?> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<EstadoSesionDto> ActualizarEstadoAsync(int usuarioId, ActualizarEstadoSesionDto dto);
}
