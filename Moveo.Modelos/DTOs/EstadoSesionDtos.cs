namespace Moveo.Modelos.DTOs;

public class EstadoSesionDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public bool EstaEnLinea { get; set; }
    public DateTime UltimaConexion { get; set; }
    public string? Dispositivo { get; set; }
}

public class CrearEstadoSesionDto
{
    public int UsuarioId { get; set; }
    public bool EstaEnLinea { get; set; }
    public string? Dispositivo { get; set; }
}

public class ActualizarEstadoSesionDto
{
    public bool EstaEnLinea { get; set; }
    public string? Dispositivo { get; set; }
}
