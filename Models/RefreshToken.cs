using System.Text.Json.Serialization;

namespace MoveoBack.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= Expires;

    public int UserId { get; set; }
    [JsonIgnore]
    public Usuario? Usuario { get; set; }
}
