using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IEstadisticaService
{
    Task<EstadisticaUsuarioDto?> ObtenerEstadisticasUsuarioAsync(int usuarioId);
    Task<IEnumerable<RankingUsuarioDto>> ObtenerRankingTopAsync(int count = 5, string? filtro = null, string? sortBy = "entregas");
    Task<EstadisticaGlobalDto> ObtenerEstadisticasGlobalesAsync();
    Task IncrementarEstadisticasAsync(int usuarioId, int puntos, decimal km, int entregas);
}
