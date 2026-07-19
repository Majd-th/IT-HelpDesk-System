using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace ITHelpDesk.API.Controllers;

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
}