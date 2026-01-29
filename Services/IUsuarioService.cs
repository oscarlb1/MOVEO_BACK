using MoveoBack.DTOs;

namespace MoveoBack.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ObtenerTodosLosUsuariosAsync();
    Task<UsuarioDto?> ObtenerUsuarioPorIdAsync(int id);
    Task<UsuarioDto> CrearUsuarioAsync(CrearUsuarioDto createUserDto);
}
