using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _userRepoMock;
    private readonly Mock<IUploadService> _uploadSvcMock;
    private readonly UsuarioService _sut;

    public UsuarioServiceTests()
    {
        _userRepoMock = new Mock<IUsuarioRepository>();
        _uploadSvcMock = new Mock<IUploadService>();
        _sut = new UsuarioService(_userRepoMock.Object, _uploadSvcMock.Object);
    }

    // ========================================================
    // ObtenerTodosLosUsuariosAsync
    // ========================================================

    [Fact]
    public async Task ObtenerTodosLosUsuariosAsync_RetornaListaDeDtos()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new() { Id = 1, Nombre = "Ana", Email = "ana@test.com", Rol = "ADMIN" },
            new() { Id = 2, Nombre = "Luis", Email = "luis@test.com", Rol = "REPARTIDOR" }
        };
        _userRepoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(usuarios);

        // Act
        var resultado = await _sut.ObtenerTodosLosUsuariosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.First().Nombre.Should().Be("Ana");
    }

    // ========================================================
    // ObtenerUsuarioPorIdAsync
    // ========================================================

    [Fact]
    public async Task ObtenerUsuarioPorIdAsync_Existente_RetornaDto()
    {
        // Arrange
        var usuario = new Usuario { Id = 1, Nombre = "Test", Email = "t@t.com", Rol = "REPARTIDOR" };
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);

        // Act
        var resultado = await _sut.ObtenerUsuarioPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(1);
        resultado.Nombre.Should().Be("Test");
    }

    [Fact]
    public async Task ObtenerUsuarioPorIdAsync_Inexistente_RetornaNull()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Usuario?)null);

        // Act
        var resultado = await _sut.ObtenerUsuarioPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    // ========================================================
    // CrearUsuarioAsync
    // ========================================================

    [Fact]
    public async Task CrearUsuarioAsync_DatosValidos_CreaConPasswordHasheado()
    {
        // Arrange
        _userRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => u.Id = 1) // Simular auto-increment
            .Returns(Task.CompletedTask);

        var usuarioCreado = new Usuario { Id = 1, Nombre = "Nuevo", Email = "nuevo@test.com", Rol = "REPARTIDOR" };
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuarioCreado);

        var dto = new CrearUsuarioDto { Nombre = "Nuevo", Email = "nuevo@test.com", Password = "Pass123", Rol = "REPARTIDOR" };

        // Act
        var resultado = await _sut.CrearUsuarioAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nombre.Should().Be("Nuevo");
        _userRepoMock.Verify(r => r.AgregarAsync(It.Is<Usuario>(u =>
            u.DebeCambiarPassword == true &&
            u.PasswordHash != "Pass123" // Debe estar hasheado
        )), Times.Once);
    }

    // ========================================================
    // ActualizarUsuarioAsync
    // ========================================================

    [Fact]
    public async Task ActualizarUsuarioAsync_UsuarioExistente_ActualizaCampos()
    {
        // Arrange
        var usuario = new Usuario { Id = 1, Nombre = "Viejo", Email = "old@test.com", Rol = "REPARTIDOR" };
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        var dto = new ActualizarUsuarioDto
        {
            Nombre = "Nuevo Nombre",
            Email = "nuevo@test.com",
            Rol = "ADMIN",
            Telefono = "666111222"
        };

        // Act
        var resultado = await _sut.ActualizarUsuarioAsync(1, dto);

        // Assert
        resultado.Should().NotBeNull();
        usuario.Nombre.Should().Be("Nuevo Nombre");
        usuario.Email.Should().Be("nuevo@test.com");
        _userRepoMock.Verify(r => r.GuardarCambiosAsync(), Times.Once);
    }

    [Fact]
    public async Task ActualizarUsuarioAsync_Inexistente_RetornaNull()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Usuario?)null);
        var dto = new ActualizarUsuarioDto { Nombre = "X", Email = "x@x.com", Rol = "ADMIN" };

        // Act
        var resultado = await _sut.ActualizarUsuarioAsync(999, dto);

        // Assert
        resultado.Should().BeNull();
    }

    // ========================================================
    // EliminarUsuarioAsync
    // ========================================================

    [Fact]
    public async Task EliminarUsuarioAsync_Existente_RetornaTrueYElimina()
    {
        // Arrange
        var usuario = new Usuario { Id = 1, Nombre = "Test" };
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.EliminarAsync(1)).Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.EliminarUsuarioAsync(1);

        // Assert
        resultado.Should().BeTrue();
        _userRepoMock.Verify(r => r.EliminarAsync(1), Times.Once);
    }

    [Fact]
    public async Task EliminarUsuarioAsync_Inexistente_RetornaFalse()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Usuario?)null);

        // Act
        var resultado = await _sut.EliminarUsuarioAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }

    // ========================================================
    // ActualizarUsuarioAsync — Ramas sin cubrir
    // ========================================================

    [Fact]
    public async Task ActualizarUsuarioAsync_ConImagen_SubeImagenYActualizaUrl()
    {
        // Arrange — Cubre la rama: if (dto.Imagen != null)
        var usuario = new Usuario { Id = 1, Nombre = "Test", Email = "t@t.com", Rol = "REPARTIDOR" };
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);
        _uploadSvcMock.Setup(u => u.UploadImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile>()))
            .ReturnsAsync("http://cdn.com/nueva-foto.jpg");

        var mockFile = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
        var dto = new ActualizarUsuarioDto
        {
            Nombre = "Test",
            Email = "t@t.com",
            Rol = "REPARTIDOR",
            Imagen = mockFile.Object
        };

        // Act
        var resultado = await _sut.ActualizarUsuarioAsync(1, dto);

        // Assert
        resultado.Should().NotBeNull();
        usuario.ImagenUrl.Should().Be("http://cdn.com/nueva-foto.jpg");
        _uploadSvcMock.Verify(u => u.UploadImageAsync(mockFile.Object), Times.Once);
    }

    [Fact]
    public async Task ActualizarUsuarioAsync_ConPasswordNuevo_HasheaPassword()
    {
        // Arrange — Cubre la rama: if (!string.IsNullOrEmpty(dto.Password))
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test",
            Email = "t@t.com",
            Rol = "REPARTIDOR",
            PasswordHash = "old-hash"
        };
        _userRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        var dto = new ActualizarUsuarioDto
        {
            Nombre = "Test",
            Email = "t@t.com",
            Rol = "REPARTIDOR",
            Password = "NuevaPassword123"
        };

        // Act
        var resultado = await _sut.ActualizarUsuarioAsync(1, dto);

        // Assert
        resultado.Should().NotBeNull();
        usuario.PasswordHash.Should().NotBe("old-hash");      // Cambió
        usuario.PasswordHash.Should().NotBe("NuevaPassword123"); // Está hasheado, no en texto plano
        BCrypt.Net.BCrypt.Verify("NuevaPassword123", usuario.PasswordHash).Should().BeTrue();
    }
}
