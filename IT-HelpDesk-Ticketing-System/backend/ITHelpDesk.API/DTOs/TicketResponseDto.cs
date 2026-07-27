public class TicketResponseDto
{
    public int Id { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // ADD THESE
    public int CategoryId { get; set; }

    public int PriorityId { get; set; }

    public int StatusId { get; set; }

    // Keep these for displaying names
    public string Category { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? ResolvedDate { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string? Solution { get; set; }
}