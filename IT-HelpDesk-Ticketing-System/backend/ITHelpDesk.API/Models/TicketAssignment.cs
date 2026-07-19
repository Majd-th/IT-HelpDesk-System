namespace ITHelpDesk.API.Models;

public class TicketAssignment
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public int AssignedToUserId { get; set; }
    public User AssignedToUser { get; set; } = null!;

    public int AssignedByUserId { get; set; }
    public User AssignedByUser { get; set; } = null!;

    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
}