using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class Priority
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}