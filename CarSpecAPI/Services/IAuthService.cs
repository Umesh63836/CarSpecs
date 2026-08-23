using CarSpecAPI.Data.Models.RequestModel;
using CarSpecAPI.Data.Models.ResponseModel;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(AdminLoginDto dto);
    Task LogoutAsync(string refreshToken);
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
    Task<RegisterDto?> RegisterAsync(RegisterDto dto);
}