using System;
using System.ComponentModel.DataAnnotations;
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
    [Required]
    public string Estado { get; set; } = "PENDIENTE";

    [Column("distanciatotalestimada")]
    public decimal DistanciaTotalEstimada { get; set; }

    [ForeignKey(nameof(ConductorId))]
    public Usuario? Conductor { get; set; }

    [ForeignKey(nameof(VehiculoId))]
    public Vehiculo? Vehiculo { get; set; }

    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
