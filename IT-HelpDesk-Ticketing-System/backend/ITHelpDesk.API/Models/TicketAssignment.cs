using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITHelpDesk.API.Models;

public class TicketAssignment
{
    public int Id { get; set; }

    [Required]
    public int TicketId { get; set; }

    [ForeignKey(nameof(TicketId))]
    public Ticket Ticket { get; set; } = null!;

    [Required]
    public int AssignedToUserId { get; set; }

    [ForeignKey(nameof(AssignedToUserId))]
    public User AssignedToUser { get; set; } = null!;

    [Required]
    public int AssignedByUserId { get; set; }

    [ForeignKey(nameof(AssignedByUserId))]
    public User AssignedByUser { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string AssignmentType { get; set; }
        = "ManagerAssignment";

    [Required]
    [MaxLength(20)]
    public string ApprovalStatus { get; set; }
        = "Approved";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime AssignedDate { get; set; }
        = DateTime.UtcNow;

    public DateTime? ReviewedDate { get; set; }

    public DateTime? UnassignedDate { get; set; }

    public bool IsActive { get; set; } = true;
}