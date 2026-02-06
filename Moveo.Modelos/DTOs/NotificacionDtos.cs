namespace Moveo.Modelos.DTOs;

public class NotificacionDto
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = "Global";
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool Leido { get; set; }
    public DateTime Fecha { get; set; }
}

public class CrearNotificacionDto
{
    public int? UsuarioId { get; set; } // Null for broadcast
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}
