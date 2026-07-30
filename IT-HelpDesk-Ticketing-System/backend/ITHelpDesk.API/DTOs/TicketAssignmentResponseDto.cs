namespace ITHelpDesk.API.DTOs;

public class TicketAssignmentResponseDto
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string TicketReference { get; set; }
        = string.Empty;

    public string TicketTitle { get; set; }
        = string.Empty;

    public int AssignedToUserId { get; set; }

    public string AssignedToUser { get; set; }
        = string.Empty;

    public int AssignedByUserId { get; set; }

    public string AssignedByUser { get; set; }
        = string.Empty;

    public string AssignmentType { get; set; }
        = string.Empty;

    public string ApprovalStatus { get; set; }
        = string.Empty;

    public string? Notes { get; set; }

    public DateTime AssignedDate { get; set; }

    public DateTime? ReviewedDate { get; set; }

    public DateTime? UnassignedDate { get; set; }

    public bool IsActive { get; set; }
}