using System;

namespace Moveo.Modelos.DTOs;

public class RutaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int ConductorId { get; set; }
    public string NombreConductor { get; set; } = string.Empty;
    public int VehiculoId { get; set; }
    public string MatriculaVehiculo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal DistanciaTotalEstimada { get; set; }
}

public class CrearRutaDto
{
    public DateTime Fecha { get; set; }
    public int ConductorId { get; set; }
    public int VehiculoId { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
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

public class ActualizarEstadoRutaDto
{
    public string NuevoEstado { get; set; } = string.Empty;
}

public class RutaEstadisticasDto
{
    public int TotalRutas { get; set; }
    public int Planificadas { get; set; }
    public int EnProgreso { get; set; }
    public int Completadas { get; set; }
    public int Canceladas { get; set; }
}

public class RutaDetalleDto : RutaDto
{
    public List<EntregaDto> Entregas { get; set; } = new();
}
