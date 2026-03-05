using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moveo.AccesoDatos.Data;
using Moveo.Negocio.Servicios;

namespace Moveo.IntegrationTests.Infrastructure;

/// <summary>
/// Factory que configura la API para tests de integración:
/// - SQLite en memoria en lugar de PostgreSQL
/// - Stubs para servicios externos (Cloudinary, Clima, IA, Distancias)
/// - JWT configurado con clave fija para generar tokens de prueba
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Mantener la conexión abierta para que SQLite in-memory no se destruya
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 1. Quitar el DbContext real (PostgreSQL)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Quitar cualquier registración previa del DbContext
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(AppDbContext));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // 2. Configurar SQLite en memoria
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // 3. Reemplazar servicios externos por stubs
            ReplaceService<IUploadService, FakeUploadService>(services);
            ReplaceService<IClimaService, FakeClimaService>(services);
            ReplaceService<IDistanciasService, FakeDistanciasService>(services);
            ReplaceService<IIaOptimizationService, FakeIaOptimizationService>(services);

            // 4. Crear la BD
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    private static void ReplaceService<TInterface, TImplementation>(IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TInterface));
        if (descriptor != null)
            services.Remove(descriptor);
        services.AddScoped<TInterface, TImplementation>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
