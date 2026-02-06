using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("estadisticausuario")]
public class EstadisticaUsuario
{
    [Column("id")]
    public int Id { get; set; }

    [Column("usuarioid")]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    [Column("puntosacumulados")]
    public int PuntosAcumulados { get; set; } = 0;

    [Column("kilometrosahorrados")]
    public decimal KilometrosAhorrados { get; set; } = 0;

    [Column("entregasexitosas")]
    public int EntregasExitosas { get; set; } = 0;
}
