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

    [ForeignKey("ClienteId")]
    public Cliente? Cliente { get; set; }

    [Column("ordenparada")]
    public int OrdenParada { get; set; }

    [Column("horaentregareal")]
    public DateTime? HoraEntregaReal { get; set; }

    [Column("fotourl")]
    public string? FotoUrl { get; set; }

    [Column("firmadigitalurl")]
    public string? FirmaDigitalUrl { get; set; }

    [Column("notas")]
    public string? Notas { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = "Pendiente"; // Pendiente, EnProgreso, Completado, Fallido

    [Column("codigoqr")]
    public string? CodigoQr { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
