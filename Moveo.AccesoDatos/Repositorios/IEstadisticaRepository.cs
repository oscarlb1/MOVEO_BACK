using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IEstadisticaRepository
{
    Task<EstadisticaUsuario?> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<EstadisticaUsuario>> ObtenerMejoresUsuariosAsync(int count, string? filtro = null, string? sortBy = "entregas");
    Task<IEnumerable<EstadisticaUsuario>> ObtenerTodasAsync();
    Task ActualizarAsync(EstadisticaUsuario estadistica);
    Task AgregarAsync(EstadisticaUsuario estadistica);
    Task IncrementarEstadisticasAsync(int usuarioId, int puntos, decimal km, int entregas);
}
