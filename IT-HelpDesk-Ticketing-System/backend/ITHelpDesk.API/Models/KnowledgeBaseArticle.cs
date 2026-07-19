using System.ComponentModel.DataAnnotations;

namespace ITHelpDesk.API.Models;

public class KnowledgeBaseArticle
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public int? ApprovedByUserId { get; set; }
    public User? ApprovedByUser { get; set; }

    public bool IsApproved { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedDate { get; set; }
}