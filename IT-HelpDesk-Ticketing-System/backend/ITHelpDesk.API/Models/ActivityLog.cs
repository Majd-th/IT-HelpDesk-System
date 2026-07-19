using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class ActivityLog
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int? TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    [Required, MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}