using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class TicketComment
{
    public int Id { get; set; }

    [Required]
    public int TicketId { get; set; }

    public Ticket Ticket { get; set; } = null!;

    [Required]
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string CommentText { get; set; } = string.Empty;

    public bool IsPrivate { get; set; }

    public bool IsManagerNote { get; set; }

    public int? ParentCommentId { get; set; }

    public TicketComment? ParentComment { get; set; }

    public ICollection<TicketComment> Replies { get; set; }
        = new List<TicketComment>();

    public DateTime CreatedDate { get; set; }
        = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
    public ICollection<TicketComment> TicketComments { get; set; }
        = new List<TicketComment>();
}