using Microsoft.AspNetCore.Http;

namespace Moveo.Modelos.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimaConexion { get; set; }
}

public class CrearUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = "REPARTIDOR";
    public IFormFile? Imagen { get; set; }
}

public class ActualizarUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; } // Opcional
    public string Rol { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public IFormFile? Imagen { get; set; }
    public string? Telefono { get; set; }
}

public class ActualizarPerfilDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? ImagenUrl { get; set; }
    public IFormFile? Imagen { get; set; }
    public string? Telefono { get; set; }
    public string? Password { get; set; } // Opcional para el usuario
}
