using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int? TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } =
        string.Empty;

    [Required]
    public string Message { get; set; } =
        string.Empty;

    [Required, MaxLength(50)]
    public string Type { get; set; } =
        string.Empty;

    public bool IsRead { get; set; } =
        false;

    public DateTime CreatedDate { get; set; } =
        DateTime.UtcNow;

    public DateTime? ReadDate { get; set; }
}