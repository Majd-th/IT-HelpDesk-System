using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class ReviewAssignmentRequestDto
{
    public bool Approved { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}