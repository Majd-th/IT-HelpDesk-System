namespace ITHelpDesk.API.DTOs;

public class AvailableTicketDto
{
    public int Id { get; set; }

    public string ReferenceNumber { get; set; }
        = string.Empty;

    public string Title { get; set; }
        = string.Empty;

    public string Description { get; set; }
        = string.Empty;

    public string Category { get; set; }
        = string.Empty;

    public string Priority { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public string CreatedBy { get; set; }
        = string.Empty;

    public DateTime CreatedDate { get; set; }

    public bool HasPendingRequest { get; set; }
}