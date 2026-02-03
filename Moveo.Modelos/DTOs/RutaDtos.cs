using System;

namespace Moveo.Modelos.DTOs;

public class RutaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ConductorId { get; set; }
    public int VehiculoId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal DistanciaTotalEstimada { get; set; }
}

public class CrearRutaDto
{
    public DateTime Fecha { get; set; }
    public int ConductorId { get; set; }
    public int VehiculoId { get; set; }
    public string Estado { get; set; } = "Planificada";
    public decimal DistanciaTotalEstimada { get; set; }
}

public class ActualizarRutaDto
{
    public DateTime Fecha { get; set; }
    public int ConductorId { get; set; }
    public int VehiculoId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal DistanciaTotalEstimada { get; set; }
}
