using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class EstadoSesionService : IEstadoSesionService
{
    private readonly IEstadoSesionRepository _estadoSesionRepository;

    public EstadoSesionService(IEstadoSesionRepository estadoSesionRepository)
    {
        _estadoSesionRepository = estadoSesionRepository;
    }

    public async Task<EstadoSesionDto?> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        var e = await _estadoSesionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        if (e == null) return null;

        return new EstadoSesionDto
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            EstaEnLinea = e.EstaEnLinea,
            UltimaConexion = e.UltimaConexion,
            Dispositivo = e.Dispositivo
        };
    }

    public async Task<EstadoSesionDto> ActualizarEstadoAsync(int usuarioId, ActualizarEstadoSesionDto dto)
    {
        var estado = await _estadoSesionRepository.ObtenerPorUsuarioIdAsync(usuarioId);

        if (estado == null)
        {
            estado = new EstadoSesion
            {
                UsuarioId = usuarioId,
                EstaEnLinea = dto.EstaEnLinea,
                UltimaConexion = DateTime.UtcNow,
                Dispositivo = dto.Dispositivo
            };
            await _estadoSesionRepository.AgregarAsync(estado);
        }
        else
        {
            estado.EstaEnLinea = dto.EstaEnLinea;
            estado.UltimaConexion = DateTime.UtcNow;
            estado.Dispositivo = dto.Dispositivo;
            await _estadoSesionRepository.GuardarCambiosAsync();
        }

        return new EstadoSesionDto
        {
            Id = estado.Id,
            UsuarioId = estado.UsuarioId,
            EstaEnLinea = estado.EstaEnLinea,
            UltimaConexion = estado.UltimaConexion,
            Dispositivo = estado.Dispositivo
        };
    }
}
