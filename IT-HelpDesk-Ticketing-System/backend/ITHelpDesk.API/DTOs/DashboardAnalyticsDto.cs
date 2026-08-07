namespace ITHelpDesk.API.DTOs;

public class DashboardAnalyticsDto
{
    public string Role { get; set; } =
        string.Empty;

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    // Tickets created during the selected period
    public int TotalTickets { get; set; }

    public int PendingReviewTickets { get; set; }

    public int OpenTickets { get; set; }

    public int AssignedTickets { get; set; }

    public int InProgressTickets { get; set; }

    public int ResolvedTickets { get; set; }

    public int ClosedTickets { get; set; }

    public int CanceledTickets { get; set; }

    // Current operational information
    public int OverdueTickets { get; set; }

    public int AvailableOpenTickets { get; set; }

    public int PendingAssignmentRequests { get; set; }

    public int ActiveAgents { get; set; }

    public double AverageResolutionHours { get; set; }

    public List<DashboardBreakdownDto>
        StatusBreakdown
    { get; set; } = [];

    public List<DashboardBreakdownDto>
        PriorityBreakdown
    { get; set; } = [];

    public List<DashboardBreakdownDto>
        CategoryBreakdown
    { get; set; } = [];

    public List<DashboardTrendPointDto>
        Trend
    { get; set; } = [];
}

public class DashboardBreakdownDto
{
    public string Name { get; set; } =
        string.Empty;

    public int Count { get; set; }
}

public class DashboardTrendPointDto
{
    public string Period { get; set; } =
        string.Empty;

    public int Created { get; set; }

    public int Resolved { get; set; }

    public int Closed { get; set; }
}