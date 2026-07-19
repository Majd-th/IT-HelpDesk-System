using ITHelpDesk.API.DTOs;

namespace ITHelpDesk.API.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequestDto request);

    Task<string?> LoginAsync(LoginRequestDto request);
}