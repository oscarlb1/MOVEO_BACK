using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class EstadisticaUsuarioService : IEstadisticaUsuarioService
{
    private readonly IEstadisticaUsuarioRepository _estadisticaUsuarioRepository;

    public EstadisticaUsuarioService(IEstadisticaUsuarioRepository estadisticaUsuarioRepository)
    {
        _estadisticaUsuarioRepository = estadisticaUsuarioRepository;
    }

    public async Task<EstadisticaUsuarioDto?> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        var e = await _estadisticaUsuarioRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        if (e == null) return null;

        return new EstadisticaUsuarioDto
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            PuntosAcumulados = e.PuntosAcumulados,
            KilometrosAhorrados = e.KilometrosAhorrados,
            EntregasExitosas = e.EntregasExitosas
        };
    }

    public async Task<EstadisticaUsuarioDto> CrearOActualizarAsync(int usuarioId, ActualizarEstadisticaUsuarioDto dto)
    {
        var estadistica = await _estadisticaUsuarioRepository.ObtenerPorUsuarioIdAsync(usuarioId);

        if (estadistica == null)
        {
            estadistica = new EstadisticaUsuario
            {
                UsuarioId = usuarioId,
                PuntosAcumulados = dto.PuntosAcumulados,
                KilometrosAhorrados = dto.KilometrosAhorrados,
                EntregasExitosas = dto.EntregasExitosas
            };
            await _estadisticaUsuarioRepository.AgregarAsync(estadistica);
        }
        else
        {
            estadistica.PuntosAcumulados = dto.PuntosAcumulados;
            estadistica.KilometrosAhorrados = dto.KilometrosAhorrados;
            estadistica.EntregasExitosas = dto.EntregasExitosas;
            await _estadisticaUsuarioRepository.GuardarCambiosAsync();
        }

        return new EstadisticaUsuarioDto
        {
            Id = estadistica.Id,
            UsuarioId = estadistica.UsuarioId,
            PuntosAcumulados = estadistica.PuntosAcumulados,
            KilometrosAhorrados = estadistica.KilometrosAhorrados,
            EntregasExitosas = estadistica.EntregasExitosas
        };
    }
}
