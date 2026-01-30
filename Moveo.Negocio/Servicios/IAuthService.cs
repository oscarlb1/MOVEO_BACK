using Moveo.Modelos.DTOs;

namespace Moveo.Negocio.Servicios;

public interface IAuthService
{
    Task<RespuestaAuthDto> RegistrarAsync(RegistroUsuarioDto registerDto);
    Task<RespuestaAuthDto> IniciarSesionAsync(LoginUsuarioDto loginDto);
    Task<RespuestaAuthDto> RefrescarTokenAsync(string refreshToken);
}
