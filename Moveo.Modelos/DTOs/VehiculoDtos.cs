using System;

namespace Moveo.Modelos.DTOs;

public class VehiculoDto
{
    public int Id { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string MarcaModelo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal CapacidadCarga { get; set; }
    public decimal ConsumoMedio { get; set; }
    public int KilometrajeActual { get; set; }
    public DateTime? FechaUltimaRevision { get; set; }
}

public class UpdateVehiculoDto
{
    public string? Matricula { get; set; }
    public string? MarcaModelo { get; set; }
    public string? Estado { get; set; }
    public decimal? CapacidadCarga { get; set; }
    public decimal? ConsumoMedio { get; set; }
    public int? KilometrajeActual { get; set; }
    public DateTime? FechaUltimaRevision { get; set; }
}
