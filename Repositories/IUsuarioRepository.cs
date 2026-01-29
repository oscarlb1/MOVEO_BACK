using MoveoBack.Models;

namespace MoveoBack.Repositories;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task AgregarAsync(Usuario usuario);
    Task<Usuario?> ObtenerPorTokenDeRefrescoAsync(string token);
    Task GuardarCambiosAsync();
}
