using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITHelpDesk.API.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public int RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(255)]
    public string? ProfileImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginDate { get; set; }
    // Navigation Properties
    public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();

    public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

    public ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();

    public ICollection<TicketAttachment> TicketAttachments { get; set; } = new List<TicketAttachment>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public ICollection<TicketAssignment> AssignedTicketsHistory { get; set; } = new List<TicketAssignment>();

    public ICollection<TicketAssignment> AssignedByHistory { get; set; } = new List<TicketAssignment>();

    public ICollection<KnowledgeBaseArticle> CreatedArticles { get; set; } = new List<KnowledgeBaseArticle>();

    public ICollection<KnowledgeBaseArticle> ApprovedArticles { get; set; } = new List<KnowledgeBaseArticle>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}