using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IEstadoSesionService
{
    Task RegistrarLoginAsync(int usuarioId, string ip, string dispositivo);
    Task RegistrarActividadAsync(int usuarioId);
    Task RegistrarLogoutAsync(int usuarioId);
    Task<IEnumerable<ResumenSesionDto>> ObtenerUsuariosActivosAsync(string? rol = null);
    Task<IEnumerable<EstadoSesionDto>> ObtenerHistorialSesionesAsync(int usuarioId);
}
