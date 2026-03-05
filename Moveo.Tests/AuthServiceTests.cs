using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _userRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IEstadoSesionService> _sesionSvcMock;
    private readonly Mock<IUploadService> _uploadSvcMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUsuarioRepository>();
        _configMock = new Mock<IConfiguration>();
        _sesionSvcMock = new Mock<IEstadoSesionService>();
        _uploadSvcMock = new Mock<IUploadService>();

        // Configurar JWT mínimo para que el servicio funcione
        _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretKeyDeMinimoTreintaYDosCaracteresParaHMAC");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _sut = new AuthService(
            _userRepoMock.Object,
            _configMock.Object,
            _sesionSvcMock.Object,
            _uploadSvcMock.Object
        );
    }

    // ========================================================
    // RegistrarAsync
    // ========================================================

    [Fact]
    public async Task RegistrarAsync_UsuarioNuevo_RetornaTokensYRolRepartidor()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorEmailAsync("nuevo@test.com")).ReturnsAsync((Usuario?)null);
        _userRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
        _sesionSvcMock.Setup(s => s.RegistrarLoginAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var dto = new RegistroUsuarioDto { Nombre = "Test", Email = "nuevo@test.com", Password = "Password123" };

        // Act
        var resultado = await _sut.RegistrarAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TokenDeAcceso.Should().NotBeNullOrEmpty();
        resultado.TokenDeRefresco.Should().NotBeNullOrEmpty();
        _userRepoMock.Verify(r => r.AgregarAsync(It.Is<Usuario>(u =>
            u.Rol == "REPARTIDOR" &&
            u.DebeCambiarPassword == true &&
            u.Email == "nuevo@test.com"
        )), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_EmailExistente_LanzaException()
    {
        // Arrange
        var usuarioExistente = new Usuario { Id = 1, Email = "existente@test.com", Nombre = "Ya existe" };
        _userRepoMock.Setup(r => r.ObtenerPorEmailAsync("existente@test.com")).ReturnsAsync(usuarioExistente);

        var dto = new RegistroUsuarioDto { Nombre = "Duplicado", Email = "existente@test.com", Password = "Pass" };

        // Act
        var accion = () => _sut.RegistrarAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("User already exists");
    }

    [Fact]
    public async Task RegistrarAsync_ConImagen_SubeImagen()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorEmailAsync(It.IsAny<string>())).ReturnsAsync((Usuario?)null);
        _userRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
        _sesionSvcMock.Setup(s => s.RegistrarLoginAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _uploadSvcMock.Setup(u => u.UploadImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile>())).ReturnsAsync("http://img.com/foto.jpg");

        var mockFile = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
        var dto = new RegistroUsuarioDto { Nombre = "Con Foto", Email = "foto@test.com", Password = "Pass", Imagen = mockFile.Object };

        // Act
        var resultado = await _sut.RegistrarAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        _uploadSvcMock.Verify(u => u.UploadImageAsync(It.IsAny<Microsoft.AspNetCore.Http.IFormFile>()), Times.Once);
    }

    // ========================================================
    // IniciarSesionAsync
    // ========================================================

    [Fact]
    public async Task IniciarSesionAsync_CredencialesValidas_RetornaTokens()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            Rol = "REPARTIDOR",
            RefreshTokens = new List<RefreshToken>()
        };
        _userRepoMock.Setup(r => r.ObtenerPorEmailAsync("test@test.com")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);
        _sesionSvcMock.Setup(s => s.RegistrarLoginAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var dto = new LoginUsuarioDto { Email = "test@test.com", Password = "CorrectPassword" };

        // Act
        var resultado = await _sut.IniciarSesionAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TokenDeAcceso.Should().NotBeNullOrEmpty();
        resultado.TokenDeRefresco.Should().NotBeNullOrEmpty();
        _sesionSvcMock.Verify(s => s.RegistrarLoginAsync(1, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task IniciarSesionAsync_PasswordIncorrecto_LanzaException()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            Rol = "REPARTIDOR"
        };
        _userRepoMock.Setup(r => r.ObtenerPorEmailAsync("test@test.com")).ReturnsAsync(usuario);

        var dto = new LoginUsuarioDto { Email = "test@test.com", Password = "WrongPassword" };

        // Act
        var accion = () => _sut.IniciarSesionAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task IniciarSesionAsync_EmailInexistente_LanzaException()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorEmailAsync("noexiste@test.com")).ReturnsAsync((Usuario?)null);
        var dto = new LoginUsuarioDto { Email = "noexiste@test.com", Password = "Any" };

        // Act
        var accion = () => _sut.IniciarSesionAsync(dto);

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials");
    }

    // ========================================================
    // RefrescarTokenAsync
    // ========================================================

    [Fact]
    public async Task RefrescarTokenAsync_TokenActivo_RevocaViejoYCreaNuevo()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Token = "valid-token",
            Expires = DateTime.UtcNow.AddDays(7),
            Revoked = false,
            FamilyId = "family-1"
        };
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test",
            Email = "test@test.com",
            Rol = "REPARTIDOR",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken> { refreshToken }
        };
        _userRepoMock.Setup(r => r.ObtenerPorTokenDeRefrescoAsync("valid-token")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.RefrescarTokenAsync("valid-token");

        // Assert
        resultado.Should().NotBeNull();
        resultado.TokenDeAcceso.Should().NotBeNullOrEmpty();
        resultado.TokenDeRefresco.Should().NotBe("valid-token"); // Nuevo token
        refreshToken.Revoked.Should().BeTrue(); // Viejo revocado
        usuario.RefreshTokens.Should().HaveCount(2); // Viejo + nuevo
    }

    [Fact]
    public async Task RefrescarTokenAsync_TokenInexistente_LanzaException()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ObtenerPorTokenDeRefrescoAsync("invalid")).ReturnsAsync((Usuario?)null);

        // Act
        var accion = () => _sut.RefrescarTokenAsync("invalid");

        // Assert
        await accion.Should().ThrowAsync<Exception>().WithMessage("Invalid token");
    }

    // ========================================================
    // CerrarSesionAsync — 0% cobertura → CRÍTICO
    // ========================================================

    [Fact]
    public async Task CerrarSesionAsync_TokenActivo_RevocaYCierraSesion()
    {
        // Arrange — Cubre camino completo: usuario encontrado → token activo → revocar
        var refreshToken = new RefreshToken
        {
            Token = "session-token",
            Expires = DateTime.UtcNow.AddDays(7),
            Revoked = false,
            FamilyId = "family-1"
        };
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test",
            Email = "test@test.com",
            PasswordHash = "hash",
            Rol = "REPARTIDOR",
            RefreshTokens = new List<RefreshToken> { refreshToken }
        };
        _userRepoMock.Setup(r => r.ObtenerPorTokenDeRefrescoAsync("session-token")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);
        _sesionSvcMock.Setup(s => s.RegistrarLogoutAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _sut.CerrarSesionAsync("session-token");

        // Assert
        refreshToken.Revoked.Should().BeTrue();
        _sesionSvcMock.Verify(s => s.RegistrarLogoutAsync(1), Times.Once);
        _userRepoMock.Verify(r => r.GuardarCambiosAsync(), Times.Once);
    }

    [Fact]
    public async Task CerrarSesionAsync_UsuarioNoEncontrado_NoHaceNada()
    {
        // Arrange — Rama defensiva: token no asociado a ningún usuario
        _userRepoMock.Setup(r => r.ObtenerPorTokenDeRefrescoAsync("unknown")).ReturnsAsync((Usuario?)null);

        // Act
        await _sut.CerrarSesionAsync("unknown");

        // Assert — No se llama a ningún servicio
        _sesionSvcMock.Verify(s => s.RegistrarLogoutAsync(It.IsAny<int>()), Times.Never);
        _userRepoMock.Verify(r => r.GuardarCambiosAsync(), Times.Never);
    }
}
