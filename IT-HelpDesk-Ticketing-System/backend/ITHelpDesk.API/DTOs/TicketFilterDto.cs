namespace ITHelpDesk.API.DTOs;

public class TicketFilterDto
{
    public int? CategoryId { get; set; }

    public int? PriorityId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAfter { get; set; }

    public DateTime? CreatedBefore { get; set; }
}