using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}