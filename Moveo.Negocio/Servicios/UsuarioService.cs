using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _userRepository;

    public UsuarioService(IUsuarioRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UsuarioDto>> ObtenerTodosLosUsuariosAsync()
    {
        var users = await _userRepository.ObtenerTodosAsync();
        return users.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email
        });
    }

    public async Task<UsuarioDto?> ObtenerUsuarioPorIdAsync(int id)
    {
        var user = await _userRepository.ObtenerPorIdAsync(id);
        if (user == null) return null;

        return new UsuarioDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email
        };
    }

    public async Task<UsuarioDto> CrearUsuarioAsync(CrearUsuarioDto createUserDto)
    {
        var user = new Usuario
        {
            Nombre = createUserDto.Nombre,
            Email = createUserDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            Rol = "REPARTIDOR",
            DebeCambiarPassword = true,
            FechaRegistro = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AgregarAsync(user);

        return new UsuarioDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email
        };
    }
}
