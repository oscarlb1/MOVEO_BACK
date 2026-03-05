using Microsoft.AspNetCore.Http;
using Moveo.Modelos.DTOs;
using Moveo.Modelos.Entidades;
using Moveo.Negocio.Servicios;

namespace Moveo.IntegrationTests.Infrastructure;

/// <summary>Stub para IUploadService — retorna URL fija sin llamar a Cloudinary.</summary>
public class FakeUploadService : IUploadService
{
    public Task<string> UploadImageAsync(IFormFile file) =>
        Task.FromResult("https://fake-cdn.com/test-image.jpg");
}

/// <summary>Stub para IClimaService — retorna clima ficticio.</summary>
public class FakeClimaService : IClimaService
{
    public Task<string> ObtenerClimaEntregasAsync(IEnumerable<Entrega> entregas) =>
        Task.FromResult("Soleado 20°C en todos los puntos");
}

/// <summary>Stub para IDistanciasService — retorna matriz ficticia.</summary>
public class FakeDistanciasService : IDistanciasService
{
    public Task<string> ObtenerMatrizDistanciasAsync(IEnumerable<Entrega> entregas) =>
        Task.FromResult("[[0,5],[5,0]]");
}

/// <summary>Stub para IIaOptimizationService — retorna optimización ficticia.</summary>
public class FakeIaOptimizationService : IIaOptimizationService
{
    public Task<OptimizacionIaResponseDto?> OptimizarRutaAsync(string prompt) =>
        Task.FromResult<OptimizacionIaResponseDto?>(new OptimizacionIaResponseDto
        {
            OrdenParadas = new List<int> { 1, 2 },
            Justificacion = "Orden óptimo calculado por stub"
        });
}
