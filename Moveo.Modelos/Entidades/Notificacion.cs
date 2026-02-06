using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("notificacion")]
public class Notificacion
{
    [Column("id")]
    public int Id { get; set; }

    [Column("usuarioid")]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [Column("leido")]
    public bool Leido { get; set; } = false;

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
