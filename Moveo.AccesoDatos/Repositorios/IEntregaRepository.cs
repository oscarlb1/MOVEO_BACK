using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IEntregaRepository
{
    Task<IEnumerable<Entrega>> ObtenerTodosAsync();
    Task<Entrega?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Entrega entrega);
    Task GuardarCambiosAsync();
}
