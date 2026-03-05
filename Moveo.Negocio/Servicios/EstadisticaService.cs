using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.Negocio.Servicios;

public class EstadisticaService : IEstadisticaService
{
    private readonly IEstadisticaRepository _estadisticaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEntregaRepository _entregaRepository;
    private readonly IRutaRepository _rutaRepository;

    public EstadisticaService(
        IEstadisticaRepository estadisticaRepository, 
        IUsuarioRepository usuarioRepository,
        IEntregaRepository entregaRepository,
        IRutaRepository rutaRepository)
    {
        _estadisticaRepository = estadisticaRepository;
        _usuarioRepository = usuarioRepository;
        _entregaRepository = entregaRepository;
        _rutaRepository = rutaRepository;
    }

    public async Task<EstadisticaHoyDto> ObtenerEstadisticasHoyUsuarioAsync(int usuarioId)
    {
        var hoy = DateTime.UtcNow.Date;
        
        // Obtenemos las rutas del usuario para hoy
        var rutas = await _rutaRepository.ObtenerMisRutasAsync(usuarioId);
        var rutasHoy = rutas.Where(r => r.Fecha.Date == hoy).ToList();
        
        if (!rutasHoy.Any())
        {
            return new EstadisticaHoyDto(0, 0, 0, "0h 0m");
        }

        var entregas = new List<Entrega>();
        foreach (var ruta in rutasHoy)
        {
            var e = await _entregaRepository.ObtenerPorRutaAsync(ruta.Id);
            entregas.AddRange(e);
        }

        int total = entregas.Count;
        int completadas = entregas.Count(e => e.Estado.Equals("Completado", StringComparison.OrdinalIgnoreCase) || 
                                              e.Estado.Equals("Entregado", StringComparison.OrdinalIgnoreCase));
        
        decimal eficiencia = total > 0 ? (decimal)completadas / total * 100 : 0;

        // Estimar tiempo en ruta: desde la primera actualización a PROGRESO hoy
        var primeraActividad = rutasHoy
            .Where(r => r.Estado != "PENDIENTE")
            .OrderBy(r => r.UpdatedAt)
            .FirstOrDefault();

        string tiempoStr = "0h 0m";
        if (primeraActividad != null)
        {
            var inicio = primeraActividad.UpdatedAt;
            var ahora = DateTime.UtcNow;
            var duracion = ahora - inicio;
            
            // Si la ruta ya terminó, podríamos usar el UpdatedAt de la última
            var ultimaActividad = rutasHoy
                .OrderByDescending(r => r.UpdatedAt)
                .First();
            
            if (ultimaActividad.Estado == "COMPLETADA")
            {
                duracion = ultimaActividad.UpdatedAt - inicio;
            }

            if (duracion.TotalMinutes < 0) duracion = TimeSpan.Zero;
            tiempoStr = $"{(int)duracion.TotalHours}h {duracion.Minutes}m";
        }

        return new EstadisticaHoyDto(total, completadas, Math.Round(eficiencia, 1), tiempoStr);
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
            EntregasTotales = u.EntregasExitosas,
            ImagenUrl = u.Usuario?.ImagenUrl
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
