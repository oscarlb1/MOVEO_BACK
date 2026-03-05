using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moveo.AccesoDatos.Data;
using Moveo.IntegrationTests.Infrastructure;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;

namespace Moveo.IntegrationTests;

/// <summary>
/// Tests de integración para la gestión de rutas:
/// Verifica el flujo completo Controller → Service → Repository → BD
/// incluyendo autorización por roles.
/// </summary>
public class RutaIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RutaIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CrearRuta_ComoAdmin_Retorna201YPersiste()
    {
        // Arrange — Insertar datos necesarios en BD
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!db.Usuarios.Any(u => u.Id == 100))
            {
                db.Usuarios.Add(new Usuario
                {
                    Id = 100,
                    Nombre = "Admin Test",
                    Email = "admin@test.com",
                    PasswordHash = "hash",
                    Rol = "ADMIN",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!db.Vehiculos.Any(v => v.Id == 1))
            {
                db.Vehiculos.Add(new Vehiculo
                {
                    Id = 1,
                    Matricula = "TEST-001",
                    MarcaModelo = "Test Car",
                    Estado = "Disponible",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenAdmin());

        var rutaDto = new CrearRutaDto
        {
            Fecha = DateTime.UtcNow,
            ConductorId = 100,
            VehiculoId = 1,
            DistanciaTotalEstimada = 150.5M
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ruta", rutaDto);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — Verificar en BD
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rutaEnBd = verifyDb.Rutas.FirstOrDefault();
        rutaEnBd.Should().NotBeNull("la ruta debe persistirse en la BD");
        rutaEnBd!.Estado.Should().Be("PENDIENTE");
        rutaEnBd.ConductorId.Should().Be(100);
    }

    [Fact]
    public async Task CrearRuta_SinAuth_Retorna401()
    {
        // Arrange — Sin token
        _client.DefaultRequestHeaders.Authorization = null;

        var rutaDto = new CrearRutaDto
        {
            Fecha = DateTime.UtcNow,
            ConductorId = 1,
            VehiculoId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ruta", rutaDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public async Task ObtenerRutas_ConAuth_Retorna200()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenRepartidor());

        // Act
        var response = await _client.GetAsync("/api/ruta");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActualizarEstado_RutaExistente_Retorna204YActualizaBD()
    {
        // Arrange — Crear ruta en BD
        int rutaId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ruta = new Ruta
            {
                Fecha = DateTime.UtcNow,
                ConductorId = 100,
                VehiculoId = 1,
                Estado = "PENDIENTE",
                DistanciaTotalEstimada = 50
            };
            db.Rutas.Add(ruta);
            await db.SaveChangesAsync();
            rutaId = ruta.Id;
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenRepartidor());

        var dto = new ActualizarEstadoRutaDto { NuevoEstado = "EN_PROGRESO" };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/ruta/{rutaId}/estado", dto);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert — Verificar cambio en BD
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rutaActualizada = verifyDb.Rutas.Find(rutaId);
        rutaActualizada!.Estado.Should().Be("EN_PROGRESO");
    }
}
