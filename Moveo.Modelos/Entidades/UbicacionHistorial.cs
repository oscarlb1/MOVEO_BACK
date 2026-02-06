using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("historialubicacion")]
public class UbicacionHistorial
{
    [Column("id")]
    public int Id { get; set; }

    [Column("rutaid")]
    public int RutaId { get; set; }

    [Column("latitud")]
    public double Latitud { get; set; }

    [Column("longitud")]
    public double Longitud { get; set; }

    [Column("fechahora")]
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
}
