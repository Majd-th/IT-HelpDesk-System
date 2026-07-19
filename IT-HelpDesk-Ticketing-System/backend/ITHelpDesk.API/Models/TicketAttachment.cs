using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class TicketAttachment
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required, MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [Required, MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
}