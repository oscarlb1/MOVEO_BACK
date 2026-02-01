using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

public class Vehiculo
{
    public int Id { get; set; }
    
    [Required]
    public string Matricula { get; set; } = string.Empty;
    
    [Required]
    public string MarcaModelo { get; set; } = string.Empty;
    
    [Required]
    public string Estado { get; set; } = "DISPONIBLE"; // DISPONIBLE, EN_RUTA, MANTENIMIENTO, etc.
    
    public decimal CapacidadCarga { get; set; }
    
    public decimal ConsumoMedio { get; set; }
    
    public int KilometrajeActual { get; set; }
    
    public DateTime? FechaUltimaRevision { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? DeletedAt { get; set; }
}
