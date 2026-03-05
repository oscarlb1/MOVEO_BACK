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
/// Tests de integración para el flujo de entregas:
/// Verifica persistencia real, transiciones de estado, y manejo de errores.
/// </summary>
public class EntregaIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public EntregaIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    /// <summary>Inserta datos base necesarios para los tests de entregas.</summary>
    private async Task SeedDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!db.Usuarios.Any(u => u.Id == 300))
        {
            db.Usuarios.Add(new Usuario
            {
                Id = 300,
                Nombre = "Conductor Entrega",
                Email = "conductor@entrega.com",
                PasswordHash = "hash",
                Rol = "REPARTIDOR",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!db.Vehiculos.Any(v => v.Id == 10))
        {
            db.Vehiculos.Add(new Vehiculo
            {
                Id = 10,
                Matricula = "ENT-001",
                MarcaModelo = "Furgoneta",
                Estado = "Disponible",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!db.Clientes.Any(c => c.Id == 50))
        {
            db.Clientes.Add(new Cliente
            {
                Id = 50,
                NombreEmpresa = "Cliente Test",
                Direccion = "Calle Test 1",
                Telefono = "600000000",
                Latitud = 41.65,
                Longitud = -0.88,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!db.Rutas.Any(r => r.Id == 500))
        {
            db.Rutas.Add(new Ruta
            {
                Id = 500,
                Fecha = DateTime.UtcNow,
                ConductorId = 300,
                VehiculoId = 10,
                Estado = "EN_PROGRESO",
                DistanciaTotalEstimada = 100
            });
        }

        await db.SaveChangesAsync();
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public async Task CrearEntrega_ConClienteReal_Retorna201YPersiste()
    {
        // Arrange
        await SeedDataAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenAdmin(300));

        var dto = new CrearEntregaDto(
            RutaId: 500,
            ClienteId: 50,
            OrdenParada: 1,
            Notas: "Entrega de integración",
            CodigoQr: "QR-INT-001"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/entregas", dto);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<EntregaDto>();
        result.Should().NotBeNull();
        result!.Estado.Should().Be("Pendiente");
        result.ClienteId.Should().Be(50);

        // Assert — Verificar persistencia
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entregaEnBd = db.Entregas.FirstOrDefault(e => e.CodigoQr == "QR-INT-001");
        entregaEnBd.Should().NotBeNull("la entrega debe persistirse en BD");
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public async Task ActualizarEstado_AEntregado_AsignaHoraEntregaRealEnBD()
    {
        // Arrange
        await SeedDataAsync();
        int entregaId;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var entrega = new Entrega
            {
                RutaId = 500,
                ClienteId = 50,
                OrdenParada = 1,
                Estado = "Pendiente",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Entregas.Add(entrega);
            await db.SaveChangesAsync();
            entregaId = entrega.Id;
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenRepartidor(300));

        var dto = new ActualizarEstadoEntregaDto(
            Estado: "Entregado",
            FotoUrl: "https://foto.com/prueba.jpg",
            FirmaDigitalUrl: "https://firma.com/firma.png",
            Notas: "Entregado al portero"
        );

        // Act
        var response = await _client.PutAsJsonAsync($"/api/entregas/{entregaId}/estado", dto);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert — Verificar que HoraEntregaReal se rellenó en BD
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entregaActualizada = verifyDb.Entregas.Find(entregaId);
        entregaActualizada!.Estado.Should().Be("Entregado");
        entregaActualizada.HoraEntregaReal.Should().NotBeNull("al marcar como Entregado se debe asignar HoraEntregaReal");
        entregaActualizada.FotoUrl.Should().Be("https://foto.com/prueba.jpg");
    }

    [Fact]
    public async Task ObtenerEntregasPorRuta_RetornaEntregasCorrectas()
    {
        // Arrange
        await SeedDataAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!db.Entregas.Any(e => e.RutaId == 500 && e.Notas == "Ruta500-Test"))
            {
                db.Entregas.Add(new Entrega
                {
                    RutaId = 500,
                    ClienteId = 50,
                    OrdenParada = 1,
                    Estado = "Pendiente",
                    Notas = "Ruta500-Test",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
        }

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenRepartidor(300));

        // Act
        var response = await _client.GetAsync("/api/entregas/rutas/500");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var entregas = await response.Content.ReadFromJsonAsync<List<EntregaDto>>();
        entregas.Should().NotBeNull();
        entregas!.Should().NotBeEmpty();
        entregas.Should().AllSatisfy(e => e.RutaId.Should().Be(500));
    }

    [Fact]
    public async Task CrearEntrega_SinAuth_Retorna401()
    {
        // Arrange — Sin token
        _client.DefaultRequestHeaders.Authorization = null;

        var dto = new CrearEntregaDto(RutaId: 1, ClienteId: 1, OrdenParada: 1, Notas: null, CodigoQr: null);

        // Act
        var response = await _client.PostAsJsonAsync("/api/entregas", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ActualizarEstado_EntregaInexistente_Retorna404()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTestHelper.TokenRepartidor());

        var dto = new ActualizarEstadoEntregaDto(Estado: "Entregado", FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        var response = await _client.PutAsJsonAsync("/api/entregas/99999/estado", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
