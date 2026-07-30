using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class ReassignTicketRequestDto
{
    [Required]
    public int NewAgentId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}