using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class AssignTicketRequestDto
{
    [Required]
    public int AgentId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}