namespace ITHelpDesk.API.DTOs;

public class AgentWorkloadDto
{
    public int AgentId { get; set; }

    public string FullName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public int ActiveTicketCount { get; set; }

    public bool IsFullyLoaded { get; set; }
}