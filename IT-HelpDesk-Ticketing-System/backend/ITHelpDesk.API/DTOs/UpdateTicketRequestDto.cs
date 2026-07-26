public class UpdateTicketRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int PriorityId { get; set; }
    public int StatusId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? Solution { get; set; }
}