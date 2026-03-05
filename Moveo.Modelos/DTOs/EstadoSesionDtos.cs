namespace Moveo.Modelos.DTOs;

public class EstadoSesionDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime UltimaConexion { get; set; }
    public string Dispositivo { get; set; } = string.Empty;
    public bool EstaActiva { get; set; }
}

public class ResumenSesionDto
{
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime? UltimaConexion { get; set; }
    public bool EstaActivo { get; set; }
    public string? ImagenUrl { get; set; }
}
