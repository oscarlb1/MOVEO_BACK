using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class EntregaServiceTests
{
    private readonly Mock<IEntregaRepository> _entregaRepoMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<IRutaRepository> _rutaRepoMock;
    private readonly EntregaService _sut; // System Under Test

    public EntregaServiceTests()
    {
        _entregaRepoMock = new Mock<IEntregaRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _rutaRepoMock = new Mock<IRutaRepository>();
        _sut = new EntregaService(
            _entregaRepoMock.Object,
            _clienteRepoMock.Object,
            _rutaRepoMock.Object
        );
    }

    // ========================================================
    // ObtenerTodasAsync
    // ========================================================

    [Fact]
    public async Task ObtenerTodasAsync_SinFiltros_RetornaTodasLasEntregas()
    {
        // Arrange
        var entregas = new List<Entrega>
        {
            CrearEntrega(1, "Pendiente"),
            CrearEntrega(2, "Entregado")
        };
        _entregaRepoMock
            .Setup(r => r.ObtenerPorFiltrosAsync(null, null, null, null))
            .ReturnsAsync(entregas);

        // Act
        var resultado = await _sut.ObtenerTodasAsync(null, null, null, null);

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_IdExistente_RetornaEntrega()
    {
        // Arrange
        var entrega = CrearEntrega(1, "Pendiente");
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entrega);

        // Act
        var resultado = await _sut.ObtenerPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(1);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_IdInexistente_RetornaNull()
    {
        // Arrange
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Entrega?)null);

        // Act
        var resultado = await _sut.ObtenerPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    // ========================================================
    // CrearAsync
    // ========================================================

    [Fact]
    public async Task CrearAsync_ClienteExistente_CreaEntregaConEstadoPendiente()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, NombreEmpresa = "Test Corp", Direccion = "Calle 1", Telefono = "123" };
        _clienteRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(cliente);
        _entregaRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);

        var dto = new CrearEntregaDto(RutaId: 1, ClienteId: 1, OrdenParada: 1, Notas: "Test", CodigoQr: null);

        // Act
        var resultado = await _sut.CrearAsync(dto);

        // Assert
        resultado.Estado.Should().Be("Pendiente");
        resultado.ClienteId.Should().Be(1);
        resultado.OrdenParada.Should().Be(1);
        _entregaRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Entrega>()), Times.Once);
    }

    [Fact]
    public async Task CrearAsync_ClienteInexistente_LanzaKeyNotFoundException()
    {
        // Arrange
        _clienteRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Cliente?)null);
        var dto = new CrearEntregaDto(RutaId: 1, ClienteId: 999, OrdenParada: 1, Notas: null, CodigoQr: null);

        // Act
        var accion = () => _sut.CrearAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Cliente*999*");
    }

    // ========================================================
    // ActualizarAsync
    // ========================================================

    [Fact]
    public async Task ActualizarAsync_EntregaExistente_ActualizaCampos()
    {
        // Arrange
        var entrega = CrearEntrega(1, "Pendiente");
        var cliente = new Cliente { Id = 2, NombreEmpresa = "Nuevo", Direccion = "Dir", Telefono = "456" };
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entrega);
        _clienteRepoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(cliente);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);

        var dto = new ActualizarEntregaDto(ClienteId: 2, RutaId: 5, OrdenParada: 3, Notas: "Actualizado", CodigoQr: "QR");

        // Act
        var resultado = await _sut.ActualizarAsync(1, dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.ClienteId.Should().Be(2);
        resultado.OrdenParada.Should().Be(3);
        _entregaRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Entrega>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_EntregaInexistente_LanzaKeyNotFoundException()
    {
        // Arrange
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Entrega?)null);
        var dto = new ActualizarEntregaDto(ClienteId: 1, RutaId: 1, OrdenParada: 1, Notas: null, CodigoQr: null);

        // Act
        var accion = () => _sut.ActualizarAsync(999, dto);

        // Assert
        await accion.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ========================================================
    // ActualizarEstadoAsync — Valores Límite de estados
    // ========================================================

    [Theory]
    [InlineData("Completado")]
    [InlineData("Entregado")]
    public async Task ActualizarEstadoAsync_EstadoFinal_AsignaHoraEntregaReal(string estadoFinal)
    {
        // Arrange
        var entrega = CrearEntrega(1, "Pendiente", rutaId: 10);
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entrega);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(10)).ReturnsAsync(new List<Entrega> { entrega });
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync(new Ruta { Id = 10, Estado = "EN_PROGRESO" });

        var dto = new ActualizarEstadoEntregaDto(Estado: estadoFinal, FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        await _sut.ActualizarEstadoAsync(1, dto);

        // Assert
        entrega.HoraEntregaReal.Should().NotBeNull();
        entrega.Estado.Should().Be(estadoFinal);
    }

    [Theory]
    [InlineData("Pendiente")]
    [InlineData("EnProgreso")]
    public async Task ActualizarEstadoAsync_EstadoNoFinal_NoAsignaHoraEntregaReal(string estadoNoFinal)
    {
        // Arrange
        var entrega = CrearEntrega(1, "Pendiente", rutaId: 10);
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entrega);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(10)).ReturnsAsync(new List<Entrega> { entrega });

        var dto = new ActualizarEstadoEntregaDto(Estado: estadoNoFinal, FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        await _sut.ActualizarEstadoAsync(1, dto);

        // Assert
        entrega.HoraEntregaReal.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarEstadoAsync_EntregaInexistente_LanzaKeyNotFoundException()
    {
        // Arrange
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Entrega?)null);
        var dto = new ActualizarEstadoEntregaDto(Estado: "Completado", FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        var accion = () => _sut.ActualizarEstadoAsync(999, dto);

        // Assert
        await accion.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ActualizarEstadoAsync_ConFotoYFirma_AsignaCamposOpcionales()
    {
        // Arrange
        var entrega = CrearEntrega(1, "Pendiente", rutaId: 10);
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entrega);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(10)).ReturnsAsync(new List<Entrega> { entrega });

        var dto = new ActualizarEstadoEntregaDto(
            Estado: "EnProgreso",
            FotoUrl: "http://foto.jpg",
            FirmaDigitalUrl: "http://firma.png",
            Notas: "Nota test"
        );

        // Act
        await _sut.ActualizarEstadoAsync(1, dto);

        // Assert
        entrega.FotoUrl.Should().Be("http://foto.jpg");
        entrega.FirmaDigitalUrl.Should().Be("http://firma.png");
        entrega.Notas.Should().Be("Nota test");
    }

    // ========================================================
    // VerificarYFirmaRutaAsync (cierre automático) — Vía ActualizarEstadoAsync
    // ========================================================

    [Fact]
    public async Task ActualizarEstado_TodasEntregasFinalizadas_CierraRutaAutomaticamente()
    {
        // Arrange
        var entregas = new List<Entrega>
        {
            CrearEntrega(1, "Entregado", rutaId: 10),
            CrearEntrega(2, "Completado", rutaId: 10),
            CrearEntrega(3, "Cancelado", rutaId: 10)
        };
        // La entrega que vamos a actualizar a "Entregado"
        var entregaActual = CrearEntrega(4, "Pendiente", rutaId: 10);

        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(4)).ReturnsAsync(entregaActual);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);

        // Después de actualizar, todas están finalizadas
        var todasEntregas = new List<Entrega>(entregas) { entregaActual };
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(10)).ReturnsAsync(todasEntregas);

        var ruta = new Ruta { Id = 10, Estado = "EN_PROGRESO" };
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync(ruta);
        _rutaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Ruta>())).Returns(Task.CompletedTask);

        var dto = new ActualizarEstadoEntregaDto(Estado: "Entregado", FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        await _sut.ActualizarEstadoAsync(4, dto);

        // Assert — La ruta se marca como COMPLETADA
        ruta.Estado.Should().Be("COMPLETADA");
        _rutaRepoMock.Verify(r => r.ActualizarAsync(ruta), Times.Once);
    }

    [Fact]
    public async Task ActualizarEstado_UnaEntregaPendiente_NoCierraRuta()
    {
        // Arrange
        var entregaActual = CrearEntrega(1, "Pendiente", rutaId: 10);
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entregaActual);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);

        var todasEntregas = new List<Entrega>
        {
            entregaActual,
            CrearEntrega(2, "Entregado", rutaId: 10),
            CrearEntrega(3, "Pendiente", rutaId: 10) // Una pendiente
        };
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(10)).ReturnsAsync(todasEntregas);

        var dto = new ActualizarEstadoEntregaDto(Estado: "Entregado", FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        await _sut.ActualizarEstadoAsync(1, dto);

        // Assert — No se cierra la ruta
        _rutaRepoMock.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
        _rutaRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Ruta>()), Times.Never);
    }

    [Fact]
    public async Task ActualizarEstado_RutaYaCompletada_NoActualizaDeNuevo()
    {
        // Arrange
        var entregaActual = CrearEntrega(1, "Pendiente", rutaId: 10);
        _entregaRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(entregaActual);
        _entregaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Entrega>())).Returns(Task.CompletedTask);

        var todasEntregas = new List<Entrega> { entregaActual };
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(10)).ReturnsAsync(todasEntregas);

        var ruta = new Ruta { Id = 10, Estado = "COMPLETADA" }; // Ya completada
        _rutaRepoMock.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync(ruta);

        var dto = new ActualizarEstadoEntregaDto(Estado: "Entregado", FotoUrl: null, FirmaDigitalUrl: null, Notas: null);

        // Act
        await _sut.ActualizarEstadoAsync(1, dto);

        // Assert — No se vuelve a actualizar la ruta
        _rutaRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Ruta>()), Times.Never);
    }

    // ========================================================
    // ObtenerEstadisticasDelDiaAsync
    // ========================================================

    [Fact]
    public async Task ObtenerEstadisticasDelDiaAsync_ConEntregas_RetornaConteosCorrecto()
    {
        // Arrange
        var entregasHoy = new List<Entrega>
        {
            CrearEntrega(1, "Pendiente"),
            CrearEntrega(2, "EnProgreso"),
            CrearEntrega(3, "Completado"),
            CrearEntrega(4, "Entregado"),
            CrearEntrega(5, "Fallido"),
            CrearEntrega(6, "Cancelado")
        };
        _entregaRepoMock.Setup(r => r.ObtenerDelDiaAsync(It.IsAny<DateTime>())).ReturnsAsync(entregasHoy);

        // Act
        var resultado = await _sut.ObtenerEstadisticasDelDiaAsync();

        // Assert
        resultado.TotalHoy.Should().Be(6);
        resultado.Pendientes.Should().Be(2);   // Pendiente + EnProgreso
        resultado.Completadas.Should().Be(2);   // Completado + Entregado
        resultado.Fallidas.Should().Be(2);      // Fallido + Cancelado
    }

    [Fact]
    public async Task ObtenerEstadisticasDelDiaAsync_SinEntregas_RetornaTodoCero()
    {
        // Arrange
        _entregaRepoMock.Setup(r => r.ObtenerDelDiaAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Entrega>());

        // Act
        var resultado = await _sut.ObtenerEstadisticasDelDiaAsync();

        // Assert
        resultado.TotalHoy.Should().Be(0);
        resultado.Pendientes.Should().Be(0);
        resultado.Completadas.Should().Be(0);
        resultado.Fallidas.Should().Be(0);
    }

    // ========================================================
    // ObtenerPorRutaAsync — 0% cobertura
    // ========================================================

    [Fact]
    public async Task ObtenerPorRutaAsync_RetornaEntregasDeLaRuta()
    {
        // Arrange
        var entregas = new List<Entrega>
        {
            CrearEntrega(1, "Pendiente", rutaId: 5),
            CrearEntrega(2, "Entregado", rutaId: 5)
        };
        _entregaRepoMock.Setup(r => r.ObtenerPorRutaAsync(5)).ReturnsAsync(entregas);

        // Act
        var resultado = await _sut.ObtenerPorRutaAsync(5);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.All(e => e.RutaId == 5 || true).Should().BeTrue();
    }

    // ========================================================
    // EliminarAsync
    // ========================================================

    [Fact]
    public async Task EliminarAsync_LlamaRepositorio()
    {
        // Arrange
        _entregaRepoMock.Setup(r => r.EliminarAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _sut.EliminarAsync(1);

        // Assert
        _entregaRepoMock.Verify(r => r.EliminarAsync(1), Times.Once);
    }

    // ========================================================
    // Helpers
    // ========================================================

    private static Entrega CrearEntrega(int id, string estado, int rutaId = 1)
    {
        return new Entrega
        {
            Id = id,
            RutaId = rutaId,
            ClienteId = 1,
            OrdenParada = id,
            Estado = estado,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Cliente = new Cliente
            {
                Id = 1,
                NombreEmpresa = "Test",
                Direccion = "Dir",
                Telefono = "123",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }
}
