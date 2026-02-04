using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IEntregaRepository
{
    Task<IEnumerable<Entrega>> ObtenerTodasAsync();
    Task<Entrega?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Entrega>> ObtenerPorRutaAsync(int rutaId);
    Task<IEnumerable<Entrega>> ObtenerPorFiltrosAsync(int? rutaId, int? clienteId, string? estado, DateTime? fecha);
    Task AgregarAsync(Entrega entrega);
    Task ActualizarAsync(Entrega entrega);
    Task EliminarAsync(int id);
    Task<IEnumerable<Entrega>> ObtenerDelDiaAsync(DateTime fecha);
    Task GuardarCambiosAsync();
}
