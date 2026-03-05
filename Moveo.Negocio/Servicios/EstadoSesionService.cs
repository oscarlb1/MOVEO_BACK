using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class EstadoSesionService : IEstadoSesionService
{
    private readonly IEstadoSesionRepository _sesionRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EstadoSesionService(IEstadoSesionRepository sesionRepository, IUsuarioRepository usuarioRepository)
    {
        _sesionRepository = sesionRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task RegistrarLoginAsync(int usuarioId, string ip, string dispositivo)
    {
        var sesion = await _sesionRepository.ObtenerActivaPorUsuarioIdAsync(usuarioId);

        if (sesion == null)
        {
            sesion = new EstadoSesion
            {
                UsuarioId = usuarioId,
                EstaEnLinea = true,
                UltimaConexion = DateTime.UtcNow,
                Dispositivo = dispositivo
            };
            await _sesionRepository.AgregarAsync(sesion);
        }
        else
        {
            sesion.EstaEnLinea = true;
            sesion.UltimaConexion = DateTime.UtcNow;
            sesion.Dispositivo = dispositivo;
            await _sesionRepository.ActualizarAsync(sesion);
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
        if (usuario != null)
        {
            usuario.UltimaConexion = DateTime.UtcNow;
            await _usuarioRepository.GuardarCambiosAsync();
        }
    }

    public async Task RegistrarActividadAsync(int usuarioId)
    {
        var sesion = await _sesionRepository.ObtenerActivaPorUsuarioIdAsync(usuarioId);
        if (sesion != null)
        {
            sesion.UltimaConexion = DateTime.UtcNow;
            sesion.EstaEnLinea = true;
            await _sesionRepository.ActualizarAsync(sesion);
        }
    }

    public async Task RegistrarLogoutAsync(int usuarioId)
    {
        var sesion = await _sesionRepository.ObtenerActivaPorUsuarioIdAsync(usuarioId);
        if (sesion != null)
        {
            sesion.EstaEnLinea = false;
            await _sesionRepository.ActualizarAsync(sesion);
        }
    }

    public async Task<IEnumerable<ResumenSesionDto>> ObtenerUsuariosActivosAsync(string? rol = null)
    {
        var usuarios = await _usuarioRepository.ObtenerTodosAsync();
        var activas = await _sesionRepository.ObtenerActivasAsync();

        var resumen = usuarios.Select(u => new ResumenSesionDto
        {
            UsuarioId = u.Id,
            NombreUsuario = u.Nombre,
            Rol = u.Rol,
            UltimaConexion = u.UltimaConexion,
            EstaActivo = activas.Any(a => a.UsuarioId == u.Id),
            ImagenUrl = u.ImagenUrl
        });

        if (!string.IsNullOrEmpty(rol))
        {
            resumen = resumen.Where(r => r.Rol.Equals(rol, StringComparison.OrdinalIgnoreCase));
        }

        return resumen.OrderByDescending(r => r.EstaActivo).ThenByDescending(r => r.UltimaConexion);
    }

    public async Task<IEnumerable<EstadoSesionDto>> ObtenerHistorialSesionesAsync(int usuarioId)
    {
        var sesiones = await _sesionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        return sesiones.Select(s => new EstadoSesionDto
        {
            Id = s.Id,
            UsuarioId = s.UsuarioId,
            NombreUsuario = s.Usuario?.Nombre ?? "Cargando...",
            Rol = s.Usuario?.Rol ?? "N/A",
            UltimaConexion = s.UltimaConexion,
            Dispositivo = s.Dispositivo ?? "N/A",
            EstaActiva = s.EstaActivoReciente
        });
    }
}
