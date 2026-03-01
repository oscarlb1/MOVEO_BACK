using System.Threading.Tasks;
using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IIaOptimizationService
{
    Task<OptimizacionIaResponseDto?> OptimizarRutaAsync(string prompt);
}
