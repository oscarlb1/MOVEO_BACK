using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Moveo.IntegrationTests.Infrastructure;

/// <summary>
/// Helper para generar tokens JWT válidos para los tests de integración.
/// Usa la misma clave de appsettings.json para que el middleware de auth los acepte.
/// </summary>
public static class JwtTestHelper
{
    // Debe coincidir con la clave en appsettings.json del proyecto API
    private const string SecretKey = "ThisIsASecretKeyForDevelopmentOnly12345!";
    private const string Issuer = "MoveoBack";
    private const string Audience = "MoveoBack";

    public static string GenerarToken(int userId, string nombre, string email, string rol)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, nombre),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, rol),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Token de ADMIN para endpoints protegidos con [Authorize(Roles = "ADMIN")]</summary>
    public static string TokenAdmin(int userId = 100) =>
        GenerarToken(userId, "Admin Test", "admin@test.com", "ADMIN");

    /// <summary>Token de REPARTIDOR para endpoints con [Authorize]</summary>
    public static string TokenRepartidor(int userId = 200) =>
        GenerarToken(userId, "Repartidor Test", "repartidor@test.com", "REPARTIDOR");
}
