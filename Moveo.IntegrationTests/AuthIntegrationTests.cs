using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moveo.AccesoDatos.Data;
using Moveo.IntegrationTests.Infrastructure;
using Moveo.Modelos.DTOs;

namespace Moveo.IntegrationTests;

/// <summary>
/// Tests de integración para el flujo de autenticación:
/// Controller → Service → Repository → SQLite (en memoria)
/// </summary>
public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public async Task Registrar_UsuarioNuevo_CreaEnBDYRetornaTokens()
    {
        // Arrange — Usamos multipart/form-data porque el endpoint usa [FromForm]
        var content = new MultipartFormDataContent
        {
            { new StringContent("Integration User"), "Nombre" },
            { new StringContent("integration@test.com"), "Email" },
            { new StringContent("SecurePass123!"), "Password" }
        };

        // Act
        var response = await _client.PostAsync("/api/auth/registrar", content);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RespuestaAuthDto>();
        result.Should().NotBeNull();
        result!.TokenDeAcceso.Should().NotBeNullOrEmpty();
        result.TokenDeRefresco.Should().NotBeNullOrEmpty();

        // Assert — Verificar persistencia en BD real
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuarioEnBd = db.Usuarios.FirstOrDefault(u => u.Email == "integration@test.com");
        usuarioEnBd.Should().NotBeNull("el usuario debe persistirse en la base de datos");
        usuarioEnBd!.Nombre.Should().Be("Integration User");
        usuarioEnBd.Rol.Should().Be("REPARTIDOR"); // Rol por defecto
        usuarioEnBd.PasswordHash.Should().NotBe("SecurePass123!"); // Debe estar hasheado
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public async Task IniciarSesion_CredencialesValidas_RetornaTokens()
    {
        // Arrange — Primero registrar un usuario
        var registerContent = new MultipartFormDataContent
        {
            { new StringContent("Login User"), "Nombre" },
            { new StringContent("login@test.com"), "Email" },
            { new StringContent("LoginPass123!"), "Password" }
        };
        await _client.PostAsync("/api/auth/registrar", registerContent);

        // Act — Intentar login
        var loginDto = new LoginUsuarioDto { Email = "login@test.com", Password = "LoginPass123!" };
        var response = await _client.PostAsJsonAsync("/api/auth/iniciar-sesion", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RespuestaAuthDto>();
        result.Should().NotBeNull();
        result!.TokenDeAcceso.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task IniciarSesion_CredencialesInvalidas_Retorna401()
    {
        // Arrange
        var loginDto = new LoginUsuarioDto { Email = "noexiste@test.com", Password = "wrong" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/iniciar-sesion", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
