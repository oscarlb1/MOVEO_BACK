using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoveoBack.DTOs;
using MoveoBack.Models;
using MoveoBack.Repositories;

namespace MoveoBack.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        var refreshToken = GenerarTokenDeRefresco(user.Id);
        user.RefreshTokens.Add(refreshToken);

        await _userRepository.AgregarAsync(user);

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
        refreshToken.Revoked = DateTime.UtcNow;

        // Generate new tokens
        var newRefreshToken = GenerarTokenDeRefresco(user.Id);
        user.RefreshTokens.Add(newRefreshToken);

        await _userRepository.GuardarCambiosAsync();

        var accessToken = GenerarTokenDeAcceso(user);

        return new RespuestaAuthDto
        {
            TokenDeAcceso = accessToken,
            TokenDeRefresco = newRefreshToken.Token
        };
    }

    private string GenerarTokenDeAcceso(Usuario user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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
            UserId = userId
        };
    }
}
