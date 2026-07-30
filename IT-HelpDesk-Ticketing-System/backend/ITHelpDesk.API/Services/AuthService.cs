using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Helpers;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using System.Security.Claims;
namespace ITHelpDesk.API.Services;

using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Models;
using ITHelpDesk.API.Data;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;
    public AuthService(
     ApplicationDbContext context,
     IUserRepository userRepository,
     JwtHelper jwtHelper,
     IEmailService emailService)
    {
        _context = context;
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
        _emailService = emailService;
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
    public async Task<UserResponseDto?> GetCurrentUserAsync(
    ClaimsPrincipal userClaims)
    {
        var userIdText = userClaims.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdText, out int userId))
            return null;

        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return null;

        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.Name,
            IsActive = user.IsActive
        };
    }
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return false;

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var resetToken = new PasswordResetToken
        {
            TokenHash = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Used = false
        };

        _context.PasswordResetTokens.Add(resetToken);

        await _context.SaveChangesAsync();

        var resetLink =
            $"http://localhost:5173/reset-password?token={token}";

        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            resetLink);

        return true;
    }



    public async Task<bool> ResetPasswordAsync(
        ResetPasswordRequestDto request)
    {
        var resetToken = await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.TokenHash == request.Token &&
                !t.Used);

        if (resetToken == null)
            return false;

        if (resetToken.ExpiresAt < DateTime.UtcNow)
            return false;

        resetToken.User.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        resetToken.Used = true;

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<UserResponseDto?> UpdateProfileAsync(
    ClaimsPrincipal userClaims,
    UpdateProfileRequestDto request)
    {
        var userIdText = userClaims.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdText, out int userId))
            return null;

        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return null;

        string normalizedEmail =
            request.Email.Trim().ToLower();

        var existingUser =
            await _userRepository.GetUserByEmailAsync(
                normalizedEmail);

        if (
            existingUser != null &&
            existingUser.Id != user.Id
        )
        {
            throw new InvalidOperationException(
                "This email address is already being used.");
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = normalizedEmail;
        user.PhoneNumber = string.IsNullOrWhiteSpace(
            request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim();

        await _userRepository.UpdateAsync(user);

        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.Name,
            IsActive = user.IsActive
        };
    }

    public async Task<(bool Success, string Message)>
        ChangePasswordAsync(
            ClaimsPrincipal userClaims,
            ChangePasswordRequestDto request)
    {
        var userIdText = userClaims.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdText, out int userId))
        {
            return (
                false,
                "The authenticated user could not be identified."
            );
        }

        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return (false, "User not found.");

        bool currentPasswordIsCorrect =
            BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash);

        if (!currentPasswordIsCorrect)
        {
            return (
                false,
                "The current password is incorrect."
            );
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return (
                false,
                "The new passwords do not match."
            );
        }

        if (request.CurrentPassword == request.NewPassword)
        {
            return (
                false,
                "The new password must be different from the current password."
            );
        }

        user.PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword);

        await _userRepository.UpdateAsync(user);

        return (
            true,
            "Password changed successfully."
        );
    }
}