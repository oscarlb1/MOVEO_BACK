using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class NotificacionServiceTests
{
    private readonly Mock<INotificacionRepository> _notificacionRepoMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly NotificacionService _sut;

    public NotificacionServiceTests()
    {
        _notificacionRepoMock = new Mock<INotificacionRepository>();
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _sut = new NotificacionService(_notificacionRepoMock.Object, _usuarioRepoMock.Object);
    }

    [Fact]
    public async Task ObtenerMisNotificacionesAsync_RetornaNotificacionesDelUsuario()
    {
        // Arrange
        var notificaciones = new List<Notificacion>
        {
            new() { Id = 1, UsuarioId = 5, Titulo = "Ruta asignada", Mensaje = "Tienes nueva ruta", Leido = false, Fecha = DateTime.UtcNow },
            new() { Id = 2, UsuarioId = 5, Titulo = "Recordatorio", Mensaje = "Revisa entregas", Leido = true, Fecha = DateTime.UtcNow }
        };
        _notificacionRepoMock.Setup(r => r.ObtenerPorUsuarioIdAsync(5)).ReturnsAsync(notificaciones);

        // Act
        var resultado = await _sut.ObtenerMisNotificacionesAsync(5);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.First().Titulo.Should().Be("Ruta asignada");
    }

    [Fact]
    public async Task EnviarNotificacionAsync_CreaNotificacion()
    {
        // Arrange
        _notificacionRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>())).Returns(Task.CompletedTask);

        var dto = new CrearNotificacionDto { UsuarioId = 3, Titulo = "Test", Mensaje = "Mensaje test" };

        // Act
        await _sut.EnviarNotificacionAsync(dto);

        // Assert
        _notificacionRepoMock.Verify(r => r.AgregarAsync(It.Is<Notificacion>(n =>
            n.UsuarioId == 3 &&
            n.Titulo == "Test" &&
            n.Leido == false
        )), Times.Once);
    }

    [Fact]
    public async Task EnviarABroadcastAsync_EnviaATodosLosUsuarios()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new() { Id = 1, Nombre = "A", Email = "a@t.com" },
            new() { Id = 2, Nombre = "B", Email = "b@t.com" },
            new() { Id = 3, Nombre = "C", Email = "c@t.com" }
        };
        _usuarioRepoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(usuarios);
        _notificacionRepoMock.Setup(r => r.AgregarVariasAsync(It.IsAny<IEnumerable<Notificacion>>())).Returns(Task.CompletedTask);

        var dto = new CrearNotificacionDto { Titulo = "Broadcast", Mensaje = "Para todos" };

        // Act
        await _sut.EnviarABroadcastAsync(dto);

        // Assert
        _notificacionRepoMock.Verify(r => r.AgregarVariasAsync(
            It.Is<IEnumerable<Notificacion>>(notifs => notifs.Count() == 3)
        ), Times.Once);
    }

    [Fact]
    public async Task MarcarComoLeidaAsync_NotificacionExistente_RetornaTrue()
    {
        // Arrange
        var notificacion = new Notificacion { Id = 1, Leido = false };
        _notificacionRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(notificacion);
        _notificacionRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Notificacion>())).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.MarcarComoLeidaAsync(1);

        // Assert
        resultado.Should().BeTrue();
        notificacion.Leido.Should().BeTrue();
    }

    [Fact]
    public async Task MarcarComoLeidaAsync_Inexistente_RetornaFalse()
    {
        // Arrange
        _notificacionRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Notificacion?)null);

        // Act
        var resultado = await _sut.MarcarComoLeidaAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerConteoNoLeidasAsync_RetornaConteo()
    {
        // Arrange
        _notificacionRepoMock.Setup(r => r.ObtenerConteoNoLeidasAsync(5)).ReturnsAsync(7);

        // Act
        var resultado = await _sut.ObtenerConteoNoLeidasAsync(5);

        // Assert
        resultado.Should().Be(7);
    }
}
