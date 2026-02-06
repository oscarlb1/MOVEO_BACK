using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class EstadisticaService : IEstadisticaService
{
    private readonly IEstadisticaRepository _estadisticaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EstadisticaService(IEstadisticaRepository estadisticaRepository, IUsuarioRepository usuarioRepository)
    {
        _estadisticaRepository = estadisticaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<EstadisticaUsuarioDto?> ObtenerEstadisticasUsuarioAsync(int usuarioId)
    {
        var stats = await _estadisticaRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        if (stats == null) return null;

        return new EstadisticaUsuarioDto
        {
            UsuarioId = stats.UsuarioId,
            NombreUsuario = stats.Usuario?.Nombre ?? "Usuario",
            PuntosAcumulados = stats.PuntosAcumulados,
            KilometrosAhorrados = stats.KilometrosAhorrados,
            EntregasTotales = stats.EntregasExitosas
        };
    }

    public async Task<IEnumerable<RankingUsuarioDto>> ObtenerRankingTopAsync(int count = 5, string? filtro = null, string? sortBy = "entregas")
    {
        var topUsers = await _estadisticaRepository.ObtenerMejoresUsuariosAsync(count, filtro, sortBy);
        int posicion = 1;

        return topUsers.Select(u => new RankingUsuarioDto
        {
            Posicion = posicion++,
            UsuarioId = u.UsuarioId,
            NombreUsuario = u.Usuario?.Nombre ?? "Usuario",
            Puntos = u.PuntosAcumulados,
            EntregasTotales = u.EntregasExitosas
        });
    }

    public async Task<EstadisticaGlobalDto> ObtenerEstadisticasGlobalesAsync()
    {
        var todas = await _estadisticaRepository.ObtenerTodasAsync();

        return new EstadisticaGlobalDto
        {
            TotalUsuarios = todas.Count(),
            TotalEntregasTotales = todas.Sum(e => e.EntregasExitosas),
            TotalKilometrosAhorrados = todas.Sum(e => e.KilometrosAhorrados),
            TotalPuntosAcumulados = todas.Sum(e => e.PuntosAcumulados)
        };
    }

    public async Task IncrementarEstadisticasAsync(int usuarioId, int puntos, decimal km, int entregas)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

        if (usuario == null)
        {
            throw new Exception("Usuario no encontrado.");
        }

        if (usuario.Rol != "REPARTIDOR")
        {
            throw new UnauthorizedAccessException("Solo los repartidores pueden acumular estadísticas.");
        }

        await _estadisticaRepository.IncrementarEstadisticasAsync(usuarioId, puntos, km, entregas);
    }
}
