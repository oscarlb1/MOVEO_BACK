using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("estado_sesion")]
public class EstadoSesion
{
    [Column("id")]
    public int Id { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    [Column("esta_en_linea")]
    public bool EstaEnLinea { get; set; } = false;

    [Column("ultima_conexion")]
    public DateTime UltimaConexion { get; set; } = DateTime.UtcNow;

    [Column("dispositivo")]
    public string? Dispositivo { get; set; }

    [NotMapped]
    public bool EstaActivoReciente => EstaEnLinea && (DateTime.UtcNow - UltimaConexion).TotalMinutes < 5;
}
