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
            Email = u.Email,
            Rol = u.Rol,
            ImagenUrl = u.ImagenUrl,
            Telefono = u.Telefono,
            FechaRegistro = u.FechaRegistro,
            UltimaConexion = u.UltimaConexion
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
            Email = user.Email,
            Rol = user.Rol,
            ImagenUrl = user.ImagenUrl,
            Telefono = user.Telefono,
            FechaRegistro = user.FechaRegistro,
            UltimaConexion = user.UltimaConexion
        };
    }

    public async Task<UsuarioDto> CrearUsuarioAsync(CrearUsuarioDto createUserDto)
    {
        var user = new Usuario
        {
            Nombre = createUserDto.Nombre,
            Email = createUserDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            Rol = createUserDto.Rol,
            DebeCambiarPassword = true,
            FechaRegistro = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AgregarAsync(user);

        return await ObtenerUsuarioPorIdAsync(user.Id) ?? throw new Exception("Error creating user");
    }

    public async Task<UsuarioDto?> ActualizarUsuarioAsync(int id, ActualizarUsuarioDto dto)
    {
        var user = await _userRepository.ObtenerPorIdAsync(id);
        if (user == null) return null;

        user.Nombre = dto.Nombre;
        user.Email = dto.Email;
        user.Rol = dto.Rol;
        user.ImagenUrl = dto.ImagenUrl;
        user.Telefono = dto.Telefono;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _userRepository.GuardarCambiosAsync();
        return await ObtenerUsuarioPorIdAsync(id);
    }

    public async Task<UsuarioDto?> ActualizarPerfilAsync(int id, ActualizarPerfilDto dto)
    {
        var user = await _userRepository.ObtenerPorIdAsync(id);
        if (user == null) return null;

        user.Nombre = dto.Nombre;
        user.ImagenUrl = dto.ImagenUrl;
        user.Telefono = dto.Telefono;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _userRepository.GuardarCambiosAsync();
        return await ObtenerUsuarioPorIdAsync(id);
    }
}
