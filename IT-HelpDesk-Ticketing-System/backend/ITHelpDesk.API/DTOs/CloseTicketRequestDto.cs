using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class CloseTicketRequestDto
{
    [Required]
    [MinLength(5)]
    [MaxLength(1000)]
    public string Reason { get; set; }
        = string.Empty;
}