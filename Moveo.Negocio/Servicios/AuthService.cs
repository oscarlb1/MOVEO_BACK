using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.AccesoDatos.Repositorios;

namespace Moveo.Negocio.Servicios;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEstadoSesionService _sesionService;
    private readonly IUploadService _uploadService;

    public AuthService(IUsuarioRepository userRepository, IConfiguration configuration, IEstadoSesionService sesionService, IUploadService uploadService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _sesionService = sesionService;
        _uploadService = uploadService;
    }

    public async Task<RespuestaAuthDto> RegistrarAsync(RegistroUsuarioDto registerDto)
    {
        if (await _userRepository.ObtenerPorEmailAsync(registerDto.Email) != null)
        {
            throw new Exception("User already exists");
        }

        var user = new Usuario
        {
            Nombre = registerDto.Nombre,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Rol = "REPARTIDOR",
            DebeCambiarPassword = true,
            FechaRegistro = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (registerDto.Imagen != null)
        {
            user.ImagenUrl = await _uploadService.UploadImageAsync(registerDto.Imagen);
        }

        var refreshToken = GenerarTokenDeRefresco(user.Id);
        user.RefreshTokens.Add(refreshToken);

        await _userRepository.AgregarAsync(user);

        // Registrar sesión inicial
        await _sesionService.RegistrarLoginAsync(user.Id, "Registro", "Web/Mobile");

        var accessToken = GenerarTokenDeAcceso(user);

        return new RespuestaAuthDto
        {
            TokenDeAcceso = accessToken,
            TokenDeRefresco = refreshToken.Token
        };
    }

    public async Task<RespuestaAuthDto> IniciarSesionAsync(LoginUsuarioDto loginDto)
    {
        var user = await _userRepository.ObtenerPorEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid credentials");
        }

        var accessToken = GenerarTokenDeAcceso(user);
        var refreshToken = GenerarTokenDeRefresco(user.Id);

        // Revoke old refresh tokens (optional, but good practice to clean up or rotate)
        // For this implementation, we just add a new one.
        user.RefreshTokens.Add(refreshToken);

        // Registrar sesión
        await _sesionService.RegistrarLoginAsync(user.Id, "Login/Auth", "Web/Mobile");

        await _userRepository.GuardarCambiosAsync();

        return new RespuestaAuthDto
        {
            TokenDeAcceso = accessToken,
            TokenDeRefresco = refreshToken.Token
        };
    }

    public async Task<RespuestaAuthDto> RefrescarTokenAsync(string token)
    {
        var user = await _userRepository.ObtenerPorTokenDeRefrescoAsync(token);
        if (user == null) throw new Exception("Invalid token");

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive) throw new Exception("Invalid token");

        // Revoke current token
        refreshToken.Revoked = true;

        // Generate new tokens
        var newRefreshToken = GenerarTokenDeRefresco(user.Id);
        newRefreshToken.FamilyId = refreshToken.FamilyId;
        user.RefreshTokens.Add(newRefreshToken);

        await _userRepository.GuardarCambiosAsync();

        var accessToken = GenerarTokenDeAcceso(user);

        return new RespuestaAuthDto
        {
            TokenDeAcceso = accessToken,
            TokenDeRefresco = newRefreshToken.Token
        };
    }

    public async Task CerrarSesionAsync(string token)
    {
        var user = await _userRepository.ObtenerPorTokenDeRefrescoAsync(token);
        if (user == null) return; // User not found, nothing to do (or throw exception depending on policy, guarding against info leak)

        var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == token);

        if (refreshToken != null && refreshToken.IsActive)
        {
            refreshToken.Revoked = true;
            await _sesionService.RegistrarLogoutAsync(user.Id);
            await _userRepository.GuardarCambiosAsync();
        }
    }

    private string GenerarTokenDeAcceso(Usuario user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.Telefono))
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.Telefono));
        }
        
        if (!string.IsNullOrEmpty(user.ImagenUrl))
        {
            claims.Add(new Claim("imagen_url", user.ImagenUrl));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private RefreshToken GenerarTokenDeRefresco(int userId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            UserId = userId,
            FamilyId = Guid.NewGuid().ToString()
        };
    }
}
