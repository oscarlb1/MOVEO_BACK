using MoveoBack.DTOs;

namespace MoveoBack.Services;

public interface IAuthService
{
    Task<RespuestaAuthDto> RegistrarAsync(RegistroUsuarioDto registerDto);
    Task<RespuestaAuthDto> IniciarSesionAsync(LoginUsuarioDto loginDto);
    Task<RespuestaAuthDto> RefrescarTokenAsync(string refreshToken);
}
