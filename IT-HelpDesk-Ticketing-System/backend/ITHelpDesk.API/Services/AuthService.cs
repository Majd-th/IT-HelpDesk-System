using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Helpers;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        // Check if email already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);

        if (existingUser != null)
            return false;

        // TODO: Later we'll replace this with a database lookup
        // var employeeRole = await _userRepository.GetRoleByNameAsync("Employee");
        // RoleId = employeeRole.Id;

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,

            // Default role = Employee
            RoleId = 4
        };

        await _userRepository.AddUserAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<string?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user == null)
            return null;

        // Prevent inactive users from logging in
        if (!user.IsActive)
            return null;

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!validPassword)
            return null;

        // Update last login time
        user.LastLoginDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        // Generate JWT
        return _jwtHelper.GenerateToken(user);
    }
}