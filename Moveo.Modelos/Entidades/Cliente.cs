using System.ComponentModel.DataAnnotations.Schema;

namespace Moveo.Modelos.Entidades;

[Table("cliente")]
public class Cliente
{
    [Column("id")]
    public int Id { get; set; }

    [Column("nombreempresa")]
    public string NombreEmpresa { get; set; } = string.Empty;

    [Column("direccion")]
    public string Direccion { get; set; } = string.Empty;

    [Column("latitud")]
    public double Latitud { get; set; }

    [Column("longitud")]
    public double Longitud { get; set; }

    [Column("telefono")]
    public string? Telefono { get; set; }
}
