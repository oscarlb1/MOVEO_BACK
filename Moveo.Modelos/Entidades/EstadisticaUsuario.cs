using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("estadisticausuario")]
public class EstadisticaUsuario
{
    [Column("id")]
    public int Id { get; set; }

    [Column("usuarioid")]
    public int UsuarioId { get; set; }

    [Column("puntosacumulados")]
    public int PuntosAcumulados { get; set; }

    [Column("kilometrosahorrados")]
    public decimal KilometrosAhorrados { get; set; }

    [Column("entregasexitosas")]
    public int EntregasExitosas { get; set; }

    public Usuario? Usuario { get; set; }
}
