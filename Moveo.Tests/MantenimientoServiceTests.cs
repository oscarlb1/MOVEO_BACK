using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class MantenimientoServiceTests
{
    private readonly Mock<IMantenimientoRepository> _mantenimientoRepoMock;
    private readonly MantenimientoService _sut;

    public MantenimientoServiceTests()
    {
        _mantenimientoRepoMock = new Mock<IMantenimientoRepository>();
        _sut = new MantenimientoService(_mantenimientoRepoMock.Object);
    }

    [Fact]
    public async Task ObtenerTodosLosMantenimientosAsync_RetornaLista()
    {
        // Arrange
        var mantenimientos = new List<Mantenimiento>
        {
            new() { Id = 1, VehiculoId = 1, TipoMantenimiento = "Aceite", Coste = 50.0m },
            new() { Id = 2, VehiculoId = 1, TipoMantenimiento = "Frenos", Coste = 200.0m }
        };
        _mantenimientoRepoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(mantenimientos);

        // Act
        var resultado = await _sut.ObtenerTodosLosMantenimientosAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerMantenimientosPorVehiculoIdAsync_RetornaFiltrados()
    {
        // Arrange
        var mantenimientos = new List<Mantenimiento>
        {
            new() { Id = 1, VehiculoId = 5, TipoMantenimiento = "Aceite" }
        };
        _mantenimientoRepoMock.Setup(r => r.ObtenerPorVehiculoIdAsync(5)).ReturnsAsync(mantenimientos);

        // Act
        var resultado = await _sut.ObtenerMantenimientosPorVehiculoIdAsync(5);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.First().VehiculoId.Should().Be(5);
    }

    [Fact]
    public async Task CrearMantenimientoAsync_DatosValidos_CreaYRetornaDto()
    {
        // Arrange
        _mantenimientoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Mantenimiento>())).Returns(Task.CompletedTask);

        var dto = new CrearMantenimientoDto
        {
            VehiculoId = 1,
            FechaServicio = DateTime.UtcNow,
            TipoMantenimiento = "Revisión general",
            KilometrajeServicio = 50000,
            Coste = 350.0m
        };

        // Act
        var resultado = await _sut.CrearMantenimientoAsync(dto);

        // Assert
        resultado.TipoMantenimiento.Should().Be("Revisión general");
        resultado.Coste.Should().Be(350.0m);
        _mantenimientoRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Mantenimiento>()), Times.Once);
    }

    [Fact]
    public async Task ActualizarMantenimientoAsync_Existente_RetornaTrue()
    {
        // Arrange
        var mantenimiento = new Mantenimiento { Id = 1, VehiculoId = 1, TipoMantenimiento = "Aceite" };
        _mantenimientoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(mantenimiento);
        _mantenimientoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Mantenimiento>())).Returns(Task.CompletedTask);

        var dto = new ActualizarMantenimientoDto
        {
            FechaServicio = DateTime.UtcNow,
            TipoMantenimiento = "Aceite + Filtro",
            KilometrajeServicio = 60000,
            Coste = 75.0m
        };

        // Act
        var resultado = await _sut.ActualizarMantenimientoAsync(1, dto);

        // Assert
        resultado.Should().BeTrue();
        mantenimiento.TipoMantenimiento.Should().Be("Aceite + Filtro");
    }

    [Fact]
    public async Task EliminarMantenimientoAsync_Inexistente_RetornaFalse()
    {
        // Arrange
        _mantenimientoRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Mantenimiento?)null);

        // Act
        var resultado = await _sut.EliminarMantenimientoAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}
