using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class EstadoSesionServiceTests
{
    private readonly Mock<IEstadoSesionRepository> _sesionRepoMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly EstadoSesionService _sut;

    public EstadoSesionServiceTests()
    {
        _sesionRepoMock = new Mock<IEstadoSesionRepository>();
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _sut = new EstadoSesionService(_sesionRepoMock.Object, _usuarioRepoMock.Object);
    }

    [Fact]
    public async Task RegistrarLoginAsync_SinSesionPrevia_CreaNuevaSesion()
    {
        // Arrange
        _sesionRepoMock.Setup(r => r.ObtenerActivaPorUsuarioIdAsync(1)).ReturnsAsync((EstadoSesion?)null);
        _sesionRepoMock.Setup(r => r.AgregarAsync(It.IsAny<EstadoSesion>())).Returns(Task.CompletedTask);
        var usuario = new Usuario { Id = 1, Nombre = "Test" };
        _usuarioRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
        _usuarioRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        // Act
        await _sut.RegistrarLoginAsync(1, "127.0.0.1", "Web");

        // Assert
        _sesionRepoMock.Verify(r => r.AgregarAsync(It.Is<EstadoSesion>(s =>
            s.UsuarioId == 1 &&
            s.EstaEnLinea == true &&
            s.Dispositivo == "Web"
        )), Times.Once);
        usuario.UltimaConexion.Should().NotBeNull();
    }

    [Fact]
    public async Task RegistrarLoginAsync_ConSesionPrevia_ActualizaSesion()
    {
        // Arrange
        var sesionExistente = new EstadoSesion { Id = 1, UsuarioId = 1, EstaEnLinea = false, Dispositivo = "Mobile" };
        _sesionRepoMock.Setup(r => r.ObtenerActivaPorUsuarioIdAsync(1)).ReturnsAsync(sesionExistente);
        _sesionRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<EstadoSesion>())).Returns(Task.CompletedTask);
        _usuarioRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Usuario { Id = 1 });
        _usuarioRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        // Act
        await _sut.RegistrarLoginAsync(1, "192.168.1.1", "Web");

        // Assert
        sesionExistente.EstaEnLinea.Should().BeTrue();
        sesionExistente.Dispositivo.Should().Be("Web");
        _sesionRepoMock.Verify(r => r.ActualizarAsync(sesionExistente), Times.Once);
    }

    [Fact]
    public async Task RegistrarLogoutAsync_ConSesionActiva_MarcaComoOffline()
    {
        // Arrange
        var sesion = new EstadoSesion { Id = 1, UsuarioId = 1, EstaEnLinea = true };
        _sesionRepoMock.Setup(r => r.ObtenerActivaPorUsuarioIdAsync(1)).ReturnsAsync(sesion);
        _sesionRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<EstadoSesion>())).Returns(Task.CompletedTask);

        // Act
        await _sut.RegistrarLogoutAsync(1);

        // Assert
        sesion.EstaEnLinea.Should().BeFalse();
        _sesionRepoMock.Verify(r => r.ActualizarAsync(sesion), Times.Once);
    }

    [Fact]
    public async Task ObtenerUsuariosActivosAsync_RetornaResumenOrdenado()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new() { Id = 1, Nombre = "Ana", Rol = "ADMIN" },
            new() { Id = 2, Nombre = "Luis", Rol = "REPARTIDOR" }
        };
        _usuarioRepoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(usuarios);

        var activas = new List<EstadoSesion>
        {
            new() { UsuarioId = 2, EstaEnLinea = true, UltimaConexion = DateTime.UtcNow }
        };
        _sesionRepoMock.Setup(r => r.ObtenerActivasAsync()).ReturnsAsync(activas);

        // Act
        var resultado = await _sut.ObtenerUsuariosActivosAsync();

        // Assert
        var lista = resultado.ToList();
        lista.Should().HaveCount(2);
        lista.First().EstaActivo.Should().BeTrue();  // Luis activo primero
        lista.First().NombreUsuario.Should().Be("Luis");
    }

    [Fact]
    public async Task ObtenerUsuariosActivosAsync_FiltradoPorRol_RetornaSoloDelRol()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new() { Id = 1, Nombre = "Ana", Rol = "ADMIN" },
            new() { Id = 2, Nombre = "Luis", Rol = "REPARTIDOR" },
            new() { Id = 3, Nombre = "María", Rol = "REPARTIDOR" }
        };
        _usuarioRepoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(usuarios);
        _sesionRepoMock.Setup(r => r.ObtenerActivasAsync()).ReturnsAsync(new List<EstadoSesion>());

        // Act
        var resultado = await _sut.ObtenerUsuariosActivosAsync("REPARTIDOR");

        // Assert
        var lista = resultado.ToList();
        lista.Should().HaveCount(2);
        lista.Should().OnlyContain(r => r.Rol == "REPARTIDOR");
    }
}
