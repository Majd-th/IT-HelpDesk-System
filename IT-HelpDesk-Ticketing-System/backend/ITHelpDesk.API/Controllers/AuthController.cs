using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace ITHelpDesk.API.Controllers;

using System.Security.Claims;
using ITHelpDesk.API.Repositories;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result)
            return BadRequest("Email already exists.");

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var token = await _authService.LoginAsync(request);

        if (token == null)
            return Unauthorized("Invalid email or password.");

        return Ok(new { Token = token });
    }
    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok("You are authenticated!");
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("Welcome Admin!");
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("manager")]
    public IActionResult ManagerOnly()
    {
        return Ok("Welcome Manager!");
    }

    [Authorize(Roles = "IT Support Agent")]
    [HttpGet("agent")]
    public IActionResult AgentOnly()
    {
        return Ok("Welcome IT Support Agent!");
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("employee")]
    public IActionResult EmployeeOnly()
    {
        return Ok("Welcome Employee!");
    }
    /*[Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Name = User.Identity?.Name,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
        });*/

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _authService.GetCurrentUserAsync(User);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    ForgotPasswordRequestDto request)
    {
        var result = await _authService.ForgotPasswordAsync(request);

        if (!result)
            return BadRequest("Email not found.");

        return Ok("Password reset link generated.");
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordRequestDto request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        if (!result)
            return BadRequest("Invalid or expired token.");

        return Ok("Password changed successfully.");
    }
}