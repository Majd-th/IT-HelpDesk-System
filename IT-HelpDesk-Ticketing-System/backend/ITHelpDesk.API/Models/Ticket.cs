using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITHelpDesk.API.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int PriorityId { get; set; }
    public Priority Priority { get; set; } = null!;

    public int StatusId { get; set; }
    public Status Status { get; set; } = null!;

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public string? Solution { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    public DateTime? ResolvedDate { get; set; }

    public DateTime? ClosedDate { get; set; }
}