namespace ITHelpDesk.API.DTOs;

public class TicketSelectionDto
{
    public int Id { get; set; }

    public string ReferenceNumber { get; set; }
        = string.Empty;

    public string Title { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public int? AssignedToUserId { get; set; }

    public string? AssignedTo { get; set; }
}