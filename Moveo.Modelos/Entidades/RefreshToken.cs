using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Moveo.Modelos.Entidades;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Column("id")]
    public int Id { get; set; }

    [Column("token_hash")]
    public string Token { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime Expires { get; set; }

    [Column("created_at")]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    [Column("revoked")]
    public bool Revoked { get; set; }

    public bool IsActive => !Revoked && !IsExpired;

    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= Expires;

    [Column("usuario_id")]
    public int UserId { get; set; }

    [Column("family_id")]
    public string FamilyId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    [JsonIgnore]
    public Usuario? Usuario { get; set; }
}
