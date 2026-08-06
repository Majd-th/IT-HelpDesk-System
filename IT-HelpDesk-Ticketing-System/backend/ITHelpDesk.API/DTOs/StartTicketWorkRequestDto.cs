using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class StartTicketWorkRequestDto
{
    [MaxLength(1000)]
    public string? Description { get; set; }
}