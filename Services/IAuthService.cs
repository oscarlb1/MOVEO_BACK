using MoveoBack.DTOs;

namespace MoveoBack.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
}
