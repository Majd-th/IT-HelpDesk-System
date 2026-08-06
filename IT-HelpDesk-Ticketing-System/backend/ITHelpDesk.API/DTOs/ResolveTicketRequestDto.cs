using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class ResolveTicketRequestDto
{
    [Required]
    [MinLength(10)]
    [MaxLength(4000)]
    public string Solution { get; set; }
        = string.Empty;

    [MaxLength(1000)]
    public string? WorkDescription { get; set; }
}