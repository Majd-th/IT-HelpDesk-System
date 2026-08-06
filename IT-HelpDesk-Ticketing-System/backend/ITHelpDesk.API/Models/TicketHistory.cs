namespace ITHelpDesk.API.Models;

public class TicketHistory
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int PerformedByUserId { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string? PreviousValue { get; set; }

    public string? NewValue { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedDate { get; set; }
        = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;

    public User PerformedByUser { get; set; } = null!;
}