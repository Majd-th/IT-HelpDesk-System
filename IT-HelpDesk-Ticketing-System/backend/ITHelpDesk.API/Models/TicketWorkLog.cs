
using ITHelpDesk.API.Models;
public class TicketWorkLog
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int AgentId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? MinutesWorked { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }
        = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;

    public User Agent { get; set; } = null!;
}