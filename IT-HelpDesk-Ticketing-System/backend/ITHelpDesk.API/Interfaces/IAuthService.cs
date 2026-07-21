using System.Security.Claims;
using ITHelpDesk.API.DTOs;

namespace ITHelpDesk.API.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequestDto request);

    Task<string?> LoginAsync(LoginRequestDto request);
    Task<UserResponseDto?> GetCurrentUserAsync(ClaimsPrincipal userClaims);
}