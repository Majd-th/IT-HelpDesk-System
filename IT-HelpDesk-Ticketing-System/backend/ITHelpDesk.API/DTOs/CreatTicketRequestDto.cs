using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.DTOs;

public class CreateTicketRequestDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; }
        = string.Empty;

    [Required]
    public string Description { get; set; }
        = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int PriorityId { get; set; }
}