using Moveo.Modelos.Entidades;

namespace Moveo.AccesoDatos.Repositorios;

public interface IRutaRepository
{
    Task<IEnumerable<Ruta>> ObtenerTodosAsync();
    Task<Ruta?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Ruta ruta);
    Task GuardarCambiosAsync();
}
