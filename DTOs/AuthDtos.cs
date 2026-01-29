namespace MoveoBack.DTOs;

public class RegistroUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginUsuarioDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RespuestaAuthDto
{
    public string TokenDeAcceso { get; set; } = string.Empty;
    public string TokenDeRefresco { get; set; } = string.Empty;
}

public class SolicitudRefrescarTokenDto
{
    public string TokenDeRefresco { get; set; } = string.Empty;
}
