using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }
}