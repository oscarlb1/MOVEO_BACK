using FluentAssertions;
using Moq;
using Moveo.AccesoDatos.Repositorios;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.Tests;

public class VehiculoServiceTests
{
    private readonly Mock<IVehiculoRepository> _vehiculoRepoMock;
    private readonly VehiculoService _sut;

    public VehiculoServiceTests()
    {
        _vehiculoRepoMock = new Mock<IVehiculoRepository>();
        _sut = new VehiculoService(_vehiculoRepoMock.Object);
    }

    [Fact]
    public async Task ObtenerTodosLosVehiculosAsync_RetornaListaDeDtos()
    {
        // Arrange
        var vehiculos = new List<Vehiculo>
        {
            new() { Id = 1, Matricula = "ABC-123", MarcaModelo = "Ford Transit", Estado = "Activo", CapacidadCarga = 1000 },
            new() { Id = 2, Matricula = "XYZ-789", MarcaModelo = "Mercedes Sprinter", Estado = "Mantenimiento", CapacidadCarga = 1500 }
        };
        _vehiculoRepoMock.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(vehiculos);

        // Act
        var resultado = await _sut.ObtenerTodosLosVehiculosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.First().Matricula.Should().Be("ABC-123");
    }

    [Fact]
    public async Task ObtenerVehiculoPorIdAsync_Existente_RetornaDto()
    {
        // Arrange
        var vehiculo = new Vehiculo { Id = 1, Matricula = "ABC-123", MarcaModelo = "Ford", Estado = "Activo" };
        _vehiculoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(vehiculo);

        // Act
        var resultado = await _sut.ObtenerVehiculoPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Matricula.Should().Be("ABC-123");
    }

    [Fact]
    public async Task CrearVehiculoAsync_DatosValidos_CreaYRetornaDto()
    {
        // Arrange
        _vehiculoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Vehiculo>())).Returns(Task.CompletedTask);
        _vehiculoRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        var dto = new CreateVehiculoDto
        {
            Matricula = "NEW-001",
            MarcaModelo = "Renault Kangoo",
            Estado = "Activo",
            CapacidadCarga = 800,
            ConsumoMedio = 7.5m,
            KilometrajeActual = 0
        };

        // Act
        var resultado = await _sut.CrearVehiculoAsync(dto);

        // Assert
        resultado.Matricula.Should().Be("NEW-001");
        resultado.MarcaModelo.Should().Be("Renault Kangoo");
        _vehiculoRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Vehiculo>()), Times.Once);
        _vehiculoRepoMock.Verify(r => r.GuardarCambiosAsync(), Times.Once);
    }

    [Fact]
    public async Task ActualizarVehiculoAsync_ActualizacionParcial_SoloActualizaCamposNoNulos()
    {
        // Arrange — Partición de Equivalencia: actualización parcial
        var vehiculo = new Vehiculo
        {
            Id = 1,
            Matricula = "OLD-001",
            MarcaModelo = "Ford Transit",
            Estado = "Activo",
            CapacidadCarga = 1000,
            ConsumoMedio = 8.0m,
            KilometrajeActual = 50000
        };
        _vehiculoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(vehiculo);
        _vehiculoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Vehiculo>())).Returns(Task.CompletedTask);
        _vehiculoRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        var dto = new UpdateVehiculoDto
        {
            Estado = "Mantenimiento",
            KilometrajeActual = 55000
            // Matricula y MarcaModelo son null → no se deben cambiar
        };

        // Act
        var resultado = await _sut.ActualizarVehiculoAsync(1, dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Estado.Should().Be("Mantenimiento");
        resultado.KilometrajeActual.Should().Be(55000);
        resultado.Matricula.Should().Be("OLD-001"); // No cambió
        resultado.MarcaModelo.Should().Be("Ford Transit"); // No cambió
    }

    [Fact]
    public async Task EliminarVehiculoAsync_Existente_RetornaTrue()
    {
        // Arrange
        var vehiculo = new Vehiculo { Id = 1, Matricula = "DEL-001" };
        _vehiculoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(vehiculo);
        _vehiculoRepoMock.Setup(r => r.EliminarAsync(1)).Returns(Task.CompletedTask);
        _vehiculoRepoMock.Setup(r => r.GuardarCambiosAsync()).Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.EliminarVehiculoAsync(1);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EliminarVehiculoAsync_Inexistente_RetornaFalse()
    {
        // Arrange
        _vehiculoRepoMock.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Vehiculo?)null);

        // Act
        var resultado = await _sut.EliminarVehiculoAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}
