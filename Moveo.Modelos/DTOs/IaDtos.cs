using System.Collections.Generic;

namespace Moveo.Modelos.DTOs;

public class OptimizacionIaResponseDto
{
    public List<int> OrdenParadas { get; set; } = new();
    public string Justificacion { get; set; } = string.Empty;
}
