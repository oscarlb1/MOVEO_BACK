using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("ruta")]
public class Ruta
{
    [Column("id")]
    public int Id { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Column("conductorid")]
    public int ConductorId { get; set; }

    [Column("vehiculoid")]
    public int VehiculoId { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = "Planificada";

    [Column("distanciatotalestimada")]
    public decimal? DistanciaTotalEstimada { get; set; }

    public Usuario? Conductor { get; set; }
    public Vehiculo? Vehiculo { get; set; }
    public List<Entrega> Entregas { get; set; } = new();
    public List<HistorialUbicacion> HistorialUbicaciones { get; set; } = new();
}
