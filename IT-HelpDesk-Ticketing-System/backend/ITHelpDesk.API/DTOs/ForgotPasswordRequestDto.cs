using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class ForgotPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}