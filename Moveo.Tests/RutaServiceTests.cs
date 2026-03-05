using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class RutaServiceTests
{
    private readonly Mock<IRutaRepository> _rutaRepoMock;
    private readonly Mock<INotificacionService> _notificacionSvcMock;
    private readonly Mock<IClimaService> _climaSvcMock;
    private readonly Mock<IDistanciasService> _distanciasSvcMock;
    private readonly Mock<IIaOptimizationService> _iaSvcMock;
    private readonly RutaService _sut;

    public RutaServiceTests()
    {
        _rutaRepoMock = new Mock<IRutaRepository>();
        _notificacionSvcMock = new Mock<INotificacionService>();
        _climaSvcMock = new Mock<IClimaService>();
        _distanciasSvcMock = new Mock<IDistanciasService>();
        _iaSvcMock = new Mock<IIaOptimizationService>();
        _sut = new RutaService(
            _rutaRepoMock.Object,
            _notificacionSvcMock.Object,
            _climaSvcMock.Object,
            _distanciasSvcMock.Object,
            _iaSvcMock.Object
        );
    }

    // ========================================================
    // ObtenerTodasAsync
    // ========================================================

    [Fact]
    public async Task ObtenerTodasAsync_RetornaListaDeRutas()
    {
        // Arrange
        var rutas = new List<Ruta>
        {
            new() { Id = 1, Estado = "PENDIENTE", Fecha = DateTime.UtcNow },
            new() { Id = 2, Estado = "EN_PROGRESO", Fecha = DateTime.UtcNow }
        };
        _rutaRepoMock.Setup(r => r.ObtenerTodasAsync(null, null, null)).ReturnsAsync(rutas);

        // Act
        var resultado = await _sut.ObtenerTodasAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_Existente_RetornaRutaDetalle()
    {
        // Arrange
        var ruta = new Ruta
        {
            Id = 1,
            Estado = "PENDIENTE",
            Fecha = DateTime.UtcNow,
            Entregas = new List<Entrega>(),
            Conductor = new Usuario { Nombre = "Juan" },
            Vehiculo = new Vehiculo { Matricula = "ABC-123" }
        };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);

        // Act
        var resultado = await _sut.ObtenerPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.NombreConductor.Should().Be("Juan");
        resultado.MatriculaVehiculo.Should().Be("ABC-123");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_Inexistente_RetornaNull()
    {
        // Arrange
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Ruta?)null);

        // Act
        var resultado = await _sut.ObtenerPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    // ========================================================
    // CrearAsync — Notificaciones al conductor
    // ========================================================

    [Fact]
    public async Task CrearAsync_ConductorValido_EnviaNotificacion()
    {
        // Arrange
        _rutaRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);
        _notificacionSvcMock.Setup(n => n.EnviarNotificacionAsync(It.IsAny<CrearNotificacionDto>())).Returns(Task.CompletedTask);

        var dto = new CrearRutaDto
        {
            Fecha = DateTime.UtcNow,
            ConductorId = 5,
            VehiculoId = 1,
            DistanciaTotalEstimada = 100
        };

        // Act
        var resultado = await _sut.CrearAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be("PENDIENTE");
        _notificacionSvcMock.Verify(
            n => n.EnviarNotificacionAsync(It.Is<CrearNotificacionDto>(d => d.UsuarioId == 5)),
            Times.Once
        );
    }

    [Fact]
    public async Task CrearAsync_ConductorIdCero_NoEnviaNotificacion()
    {
        // Arrange — Valor límite: ConductorId = 0
        _rutaRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);

        var dto = new CrearRutaDto
        {
            Fecha = DateTime.UtcNow,
            ConductorId = 0,
            VehiculoId = 1,
            DistanciaTotalEstimada = 50
        };

        // Act
        await _sut.CrearAsync(dto);

        // Assert
        _notificacionSvcMock.Verify(
            n => n.EnviarNotificacionAsync(It.IsAny<CrearNotificacionDto>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CrearAsync_FechaSinKindUtc_ConvierteAUtc()
    {
        // Arrange — Valor límite: DateTimeKind.Unspecified
        _rutaRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Ruta>()))
            .Callback<Ruta>(r => r.Fecha.Kind.Should().Be(DateTimeKind.Utc))
            .Returns(Task.CompletedTask);

        var dto = new CrearRutaDto
        {
            Fecha = new DateTime(2026, 3, 5, 10, 0, 0, DateTimeKind.Unspecified), // Sin Kind
            ConductorId = 0,
            VehiculoId = 1
        };

        // Act
        await _sut.CrearAsync(dto);

        // Assert — Verificado en el Callback
        _rutaRepoMock.Verify(r => r.AgregarAsync(It.Is<Ruta>(ruta => ruta.Fecha.Kind == DateTimeKind.Utc)), Times.Once);
    }

    // ========================================================
    // ActualizarAsync — Notificaciones por reasignación
    // ========================================================

    [Fact]
    public async Task ActualizarAsync_CambiaConductor_EnviaNotificacion()
    {
        // Arrange
        var ruta = new Ruta { Id = 1, ConductorId = 3, Estado = "PENDIENTE", Fecha = DateTime.UtcNow };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);
        _rutaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);

        var dto = new ActualizarRutaDto
        {
            Fecha = DateTime.UtcNow,
            ConductorId = 7, // Conductor diferente
            VehiculoId = 1,
            DistanciaTotalEstimada = 80
        };

        // Act
        var resultado = await _sut.ActualizarAsync(1, dto);

        // Assert
        resultado.Should().BeTrue();
        _notificacionSvcMock.Verify(
            n => n.EnviarNotificacionAsync(It.Is<CrearNotificacionDto>(d => d.UsuarioId == 7)),
            Times.Once
        );
    }

    [Fact]
    public async Task ActualizarAsync_MismoConductor_NoEnviaNotificacion()
    {
        // Arrange
        var ruta = new Ruta { Id = 1, ConductorId = 3, Estado = "PENDIENTE", Fecha = DateTime.UtcNow };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);
        _rutaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);

        var dto = new ActualizarRutaDto
        {
            Fecha = DateTime.UtcNow,
            ConductorId = 3, // Mismo conductor
            VehiculoId = 1,
            DistanciaTotalEstimada = 80
        };

        // Act
        await _sut.ActualizarAsync(1, dto);

        // Assert
        _notificacionSvcMock.Verify(
            n => n.EnviarNotificacionAsync(It.IsAny<CrearNotificacionDto>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ActualizarAsync_RutaInexistente_RetornaFalse()
    {
        // Arrange
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Ruta?)null);
        var dto = new ActualizarRutaDto { Fecha = DateTime.UtcNow, ConductorId = 1, VehiculoId = 1 };

        // Act
        var resultado = await _sut.ActualizarAsync(999, dto);

        // Assert
        resultado.Should().BeFalse();
    }

    // ========================================================
    // OptimizarRutaAsync
    // ========================================================

    [Fact]
    public async Task OptimizarRutaAsync_RutaInexistente_LanzaException()
    {
        // Arrange
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Ruta?)null);

        // Act
        var accion = () => _sut.OptimizarRutaAsync(999);

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("*no encontrada*");
    }

    [Fact]
    public async Task OptimizarRutaAsync_SinEntregas_LanzaException()
    {
        // Arrange
        var ruta = new Ruta { Id = 1, Estado = "PENDIENTE", Entregas = new List<Entrega>() };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);

        // Act
        var accion = () => _sut.OptimizarRutaAsync(1);

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("*no tiene entregas*");
    }

    [Fact]
    public async Task OptimizarRutaAsync_ConEntregas_AplicaOrdenOptimizado()
    {
        // Arrange
        var entregas = new List<Entrega>
        {
            new() { Id = 10, OrdenParada = 1, ClienteId = 1, Estado = "Pendiente",
                     Cliente = new Cliente { Id = 1, Direccion = "Dir1", Latitud = 41.6, Longitud = -0.9 } },
            new() { Id = 20, OrdenParada = 2, ClienteId = 2, Estado = "Pendiente",
                     Cliente = new Cliente { Id = 2, Direccion = "Dir2", Latitud = 41.7, Longitud = -0.8 } }
        };
        var ruta = new Ruta { Id = 1, Estado = "PENDIENTE", Entregas = entregas };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);
        _climaSvcMock.Setup(c => c.ObtenerClimaEntregasAsync(It.IsAny<IEnumerable<Entrega>>())).ReturnsAsync("Soleado 20°C");
        _distanciasSvcMock.Setup(d => d.ObtenerMatrizDistanciasAsync(It.IsAny<IEnumerable<Entrega>>())).ReturnsAsync("[[0,5],[5,0]]");
        _iaSvcMock.Setup(i => i.OptimizarRutaAsync(It.IsAny<string>()))
            .ReturnsAsync(new OptimizacionIaResponseDto { OrdenParadas = new List<int> { 20, 10 }, Justificacion = "Más eficiente" });
        _rutaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.OptimizarRutaAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado.OrdenParadas.Should().ContainInOrder(20, 10);
        entregas.First(e => e.Id == 20).OrdenParada.Should().Be(1);
        entregas.First(e => e.Id == 10).OrdenParada.Should().Be(2);
        _rutaRepoMock.Verify(r => r.ActualizarAsync(ruta), Times.Once);
    }

    // ========================================================
    // EliminarAsync / EstadísticasAsync
    // ========================================================

    [Fact]
    public async Task EliminarAsync_RutaExistente_RetornaTrue()
    {
        // Arrange
        var ruta = new Ruta { Id = 1, Estado = "PENDIENTE" };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);
        _rutaRepoMock.Setup(r => r.EliminarAsync(ruta)).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.EliminarAsync(1);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerEstadisticasAsync_RetornaConteosPorEstado()
    {
        // Arrange
        _rutaRepoMock.Setup(r => r.ObtenerTodasAsync(null, null, null)).ReturnsAsync(new List<Ruta> { new(), new(), new() });
        _rutaRepoMock.Setup(r => r.ObtenerConteoPorEstadoAsync("PENDIENTE")).ReturnsAsync(1);
        _rutaRepoMock.Setup(r => r.ObtenerConteoPorEstadoAsync("EN_PROGRESO")).ReturnsAsync(1);
        _rutaRepoMock.Setup(r => r.ObtenerConteoPorEstadoAsync("COMPLETADA")).ReturnsAsync(1);
        _rutaRepoMock.Setup(r => r.ObtenerConteoPorEstadoAsync("CANCELADA")).ReturnsAsync(0);

        // Act
        var resultado = await _sut.ObtenerEstadisticasAsync();

        // Assert
        resultado.TotalRutas.Should().Be(3);
        resultado.Planificadas.Should().Be(1);
        resultado.EnProgreso.Should().Be(1);
        resultado.Completadas.Should().Be(1);
        resultado.Canceladas.Should().Be(0);
    }

    // ========================================================
    // ActualizarEstadoAsync — 0% cobertura
    // ========================================================

    [Fact]
    public async Task ActualizarEstadoAsync_RutaExistente_CambiaEstadoYRetornaTrue()
    {
        // Arrange — Cubre el camino completo de cambio de estado directo
        var ruta = new Ruta { Id = 1, Estado = "PENDIENTE", Fecha = DateTime.UtcNow };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(ruta);
        _rutaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.ActualizarEstadoAsync(1, "EN_PROGRESO");

        // Assert
        resultado.Should().BeTrue();
        ruta.Estado.Should().Be("EN_PROGRESO");
        _rutaRepoMock.Verify(r => r.ActualizarAsync(ruta), Times.Once);
    }

    [Fact]
    public async Task ActualizarEstadoAsync_RutaInexistente_RetornaFalse()
    {
        // Arrange
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Ruta?)null);

        // Act
        var resultado = await _sut.ActualizarEstadoAsync(999, "COMPLETADA");

        // Assert
        resultado.Should().BeFalse();
    }
}
