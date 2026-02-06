using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ObtenerTodosLosUsuariosAsync();
    Task<UsuarioDto?> ObtenerUsuarioPorIdAsync(int id);
    Task<UsuarioDto> CrearUsuarioAsync(CrearUsuarioDto createUserDto);
    Task<UsuarioDto?> ActualizarUsuarioAsync(int id, ActualizarUsuarioDto updateUserDto);
    Task<UsuarioDto?> ActualizarPerfilAsync(int id, ActualizarPerfilDto updateProfileDto);
    Task<bool> EliminarUsuarioAsync(int id);
}
