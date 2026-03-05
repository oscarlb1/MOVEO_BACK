using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class EstadisticaServiceTests
{
    private readonly Mock<IEstadisticaRepository> _estadisticaRepoMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<IEntregaRepository> _entregaRepoMock;
    private readonly Mock<IRutaRepository> _rutaRepoMock;
    private readonly EstadisticaService _sut;

    public EstadisticaServiceTests()
    {
        _estadisticaRepoMock = new Mock<IEstadisticaRepository>();
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _entregaRepoMock = new Mock<IEntregaRepository>();
        _rutaRepoMock = new Mock<IRutaRepository>();
        _sut = new EstadisticaService(
            _estadisticaRepoMock.Object,
            _usuarioRepoMock.Object,
            _entregaRepoMock.Object,
            _rutaRepoMock.Object
        );
    }

    // ========================================================
    // ObtenerEstadisticasHoyUsuarioAsync
    // ========================================================

    [Fact]
    public async Task ObtenerEstadisticasHoyUsuarioAsync_SinRutasHoy_RetornaCeros()
    {
        // Arrange — Valor Límite: sin rutas para hoy
        _rutaRepoMock.Setup(r => r.ObtenerMisRutasAsync(1)).ReturnsAsync(new List<Ruta>());

        // Act
        var resultado = await _sut.ObtenerEstadisticasHoyUsuarioAsync(1);

        // Assert
        resultado.EntregasTotales.Should().Be(0);
        resultado.EntregasCompletadas.Should().Be(0);
        resultado.Eficiencia.Should().Be(0);
        resultado.TiempoEnRuta.Should().Be("0h 0m");
    }

    [Fact]
    public async Task ObtenerEstadisticasHoyUsuarioAsync_ConEntregas_CalculaEficiencia()
    {
        // Arrange
        var hoy = DateTime.UtcNow.Date;
        var rutas = new List<Ruta>
        {
            new() { Id = 1, Fecha = hoy, Estado = "EN_PROGRESO", UpdatedAt = DateTime.UtcNow.AddHours(-2) }
        };
        _rutaRepoMock.Setup(r => r.ObtenerMisRutasAsync(1)).ReturnsAsync(rutas);

        var entregas = new List<Entrega>
        {
            new() { Id = 1, Estado = "Entregado" },
            new() { Id = 2, Estado = "Completado" },
            new() { Id = 3, Estado = "Pendiente" },
            new() { Id = 4, Estado = "Fallido" }
        };
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(1)).ReturnsAsync(entregas);

        // Act
        var resultado = await _sut.ObtenerEstadisticasHoyUsuarioAsync(1);

        // Assert
        resultado.EntregasTotales.Should().Be(4);
        resultado.EntregasCompletadas.Should().Be(2); // Entregado + Completado
        resultado.Eficiencia.Should().Be(50.0m);      // 2/4 * 100
    }

    [Fact]
    public async Task ObtenerEstadisticasHoyUsuarioAsync_RutaCompletada_CalculaDuracionDesdeActualizaciones()
    {
        // Arrange
        var hoy = DateTime.UtcNow.Date;
        var inicio = hoy.AddHours(8);
        var fin = hoy.AddHours(10).AddMinutes(30);

        var rutas = new List<Ruta>
        {
            new() { Id = 1, Fecha = hoy, Estado = "COMPLETADA", UpdatedAt = fin },
            new() { Id = 2, Fecha = hoy, Estado = "EN_PROGRESO", UpdatedAt = inicio }
        };
        _rutaRepoMock.Setup(r => r.ObtenerMisRutasAsync(1)).ReturnsAsync(rutas);
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(It.IsAny<int>())).ReturnsAsync(new List<Entrega>
        {
            new() { Id = 1, Estado = "Entregado" }
        });

        // Act
        var resultado = await _sut.ObtenerEstadisticasHoyUsuarioAsync(1);

        // Assert
        resultado.TiempoEnRuta.Should().NotBe("0h 0m");
    }

    // ========================================================
    // ObtenerEstadisticasUsuarioAsync
    // ========================================================

    [Fact]
    public async Task ObtenerEstadisticasUsuarioAsync_UsuarioConDatos_RetornaDto()
    {
        // Arrange
        var stats = new EstadisticaUsuario
        {
            UsuarioId = 1,
            PuntosAcumulados = 500,
            KilometrosAhorrados = 123.5m,
            EntregasExitosas = 50,
            Usuario = new Usuario { Nombre = "Juan" }
        };
        _estadisticaRepoMock.Setup(r => r.ObtenerPorUsuarioIdAsync(1)).ReturnsAsync(stats);

        // Act
        var resultado = await _sut.ObtenerEstadisticasUsuarioAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.PuntosAcumulados.Should().Be(500);
        resultado.KilometrosAhorrados.Should().Be(123.5m);
        resultado.EntregasTotales.Should().Be(50);
        resultado.NombreUsuario.Should().Be("Juan");
    }

    [Fact]
    public async Task ObtenerEstadisticasUsuarioAsync_SinDatos_RetornaNull()
    {
        // Arrange
        _estadisticaRepoMock.Setup(r => r.ObtenerPorUsuarioIdAsync(999)).ReturnsAsync((EstadisticaUsuario?)null);

        // Act
        var resultado = await _sut.ObtenerEstadisticasUsuarioAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    // ========================================================
    // IncrementarEstadisticasAsync — Control de acceso por rol
    // ========================================================

    [Fact]
    public async Task IncrementarEstadisticasAsync_Repartidor_IncrementaCorrectamente()
    {
        // Arrange — Partición de Equivalencia: rol válido
        var usuario = new Usuario { Id = 1, Nombre = "Repartidor", Rol = "REPARTIDOR" };
        _usuarioRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
        _estadisticaRepoMock.Setup(r => r.IncrementarEstadisticasAsync(1, 10, 5.0m, 3)).Returns(Task.CompletedTask);

        // Act
        await _sut.IncrementarEstadisticasAsync(1, 10, 5.0m, 3);

        // Assert
        _estadisticaRepoMock.Verify(r => r.IncrementarEstadisticasAsync(1, 10, 5.0m, 3), Times.Once);
    }

    [Fact]
    public async Task IncrementarEstadisticasAsync_Admin_LanzaUnauthorizedAccessException()
    {
        // Arrange — Partición de Equivalencia: rol inválido
        var usuario = new Usuario { Id = 1, Nombre = "Admin", Rol = "ADMIN" };
        _usuarioRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);

        // Act
        var accion = () => _sut.IncrementarEstadisticasAsync(1, 10, 5.0m, 3);

        // Assert
        await accion.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*repartidores*");
    }

    [Fact]
    public async Task IncrementarEstadisticasAsync_UsuarioInexistente_LanzaException()
    {
        // Arrange — Valor Límite: ID que no existe
        _usuarioRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Usuario?)null);

        // Act
        var accion = () => _sut.IncrementarEstadisticasAsync(999, 10, 5.0m, 3);

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("*no encontrado*");
    }

    // ========================================================
    // ObtenerEstadisticasGlobalesAsync
    // ========================================================

    [Fact]
    public async Task ObtenerEstadisticasGlobalesAsync_RetornaTotalesAgregados()
    {
        // Arrange
        var todas = new List<EstadisticaUsuario>
        {
            new() { UsuarioId = 1, PuntosAcumulados = 100, KilometrosAhorrados = 50, EntregasExitosas = 20 },
            new() { UsuarioId = 2, PuntosAcumulados = 200, KilometrosAhorrados = 80, EntregasExitosas = 30 }
        };
        _estadisticaRepoMock.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(todas);

        // Act
        var resultado = await _sut.ObtenerEstadisticasGlobalesAsync();

        // Assert
        resultado.TotalUsuarios.Should().Be(2);
        resultado.TotalPuntosAcumulados.Should().Be(300);
        resultado.TotalKilometrosAhorrados.Should().Be(130);
        resultado.TotalEntregasTotales.Should().Be(50);
    }
}
