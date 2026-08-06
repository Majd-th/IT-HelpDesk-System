using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class PublishTicketRequestDto
{
    [Required]
    [MinLength(5)]
    [MaxLength(1000)]
    public string Notes { get; set; }
        = string.Empty;
}