using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("mantenimiento")]
public class Mantenimiento
{
    [Column("id")]
    public int Id { get; set; }

    [Column("vehiculoid")]
    public int VehiculoId { get; set; }

    [Column("fechaservicio")]
    public DateTime FechaServicio { get; set; }
    
    [Column("tipomantenimiento")]
    public string TipoMantenimiento { get; set; } = string.Empty;

    [Column("kilometrajeservicio")]
    public int KilometrajeServicio { get; set; }

    [Column("coste")]
    public decimal Coste { get; set; }

    [ForeignKey(nameof(VehiculoId))]
    public Vehiculo? Vehiculo { get; set; }
}
