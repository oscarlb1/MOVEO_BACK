using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("entrega")]
public class Entrega
{
    [Column("id")]
    public int Id { get; set; }

    [Column("rutaid")]
    public int RutaId { get; set; }

    [Column("clienteid")]
    public int ClienteId { get; set; }

    [Column("ordenparada")]
    public int OrdenParada { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = "Pendiente";

    [Column("codigoqr")]
    public string? CodigoQR { get; set; }

    [Column("firmadigitalurl")]
    public string? FirmaDigitalUrl { get; set; }

    [Column("fotourl")]
    public string? FotoUrl { get; set; }

    [Column("notas")]
    public string? Notas { get; set; }

    [Column("horaentregareal")]
    public DateTime? HoraEntregaReal { get; set; }

    public Ruta? Ruta { get; set; }
    public Cliente? Cliente { get; set; }
}
