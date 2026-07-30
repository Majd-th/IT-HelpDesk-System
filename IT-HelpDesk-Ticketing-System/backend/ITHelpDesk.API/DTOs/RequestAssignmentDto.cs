using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class RequestAssignmentDto
{
    [MaxLength(500)]
    public string? Notes { get; set; }
}