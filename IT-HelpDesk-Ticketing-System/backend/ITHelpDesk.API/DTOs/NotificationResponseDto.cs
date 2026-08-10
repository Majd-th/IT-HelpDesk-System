namespace ITHelpDesk.API.DTOs;

public class NotificationResponseDto
{
    public int Id { get; set; }

    public int? TicketId { get; set; }

    public string? TicketReference { get; set; }

    public string Title { get; set; }
        = string.Empty;

    public string Message { get; set; }
        = string.Empty;

    public string Type { get; set; }
        = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ReadDate { get; set; }
}