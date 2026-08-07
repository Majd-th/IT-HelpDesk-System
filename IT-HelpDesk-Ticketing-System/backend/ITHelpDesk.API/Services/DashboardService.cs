using ITHelpDesk.API.Constants;
using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Services;

public class DashboardService
    : IDashboardService
{
    private readonly IDashboardRepository
        _dashboardRepository;

    public DashboardService(
        IDashboardRepository dashboardRepository)
    {
        _dashboardRepository =
            dashboardRepository;
    }

    public async Task<DashboardAnalyticsDto>
        GetAnalyticsAsync(
            int userId,
            string role,
            DateTime from,
            DateTime to)
    {
        DateTime startDate =
            from.Date;

        /*
         * The API receives an inclusive "to" date.
         * Internally, use the start of the next day.
         */
        DateTime endExclusive =
            to.Date.AddDays(1);

        var allVisibleTickets =
            await _dashboardRepository
                .GetVisibleTicketsAsync(
                    userId,
                    role
                );

        /*
         * KPI and chart breakdown tickets:
         * tickets created during the selected period.
         */
        var periodTickets =
            allVisibleTickets
                .Where(ticket =>
                    ticket.CreatedDate >=
                        startDate
                    &&
                    ticket.CreatedDate <
                        endExclusive
                )
                .ToList();

        /*
         * Resolution-time calculation:
         * tickets resolved during the selected period.
         */
        var resolvedDuringPeriod =
            allVisibleTickets
                .Where(ticket =>
                    ticket.ResolvedDate.HasValue
                    &&
                    ticket.ResolvedDate.Value >=
                        startDate
                    &&
                    ticket.ResolvedDate.Value <
                        endExclusive
                )
                .ToList();

        double averageResolutionHours =
            resolvedDuringPeriod.Count == 0
                ? 0
                : resolvedDuringPeriod
                    .Average(ticket =>
                        (
                            ticket.ResolvedDate!.Value
                            -
                            ticket.CreatedDate
                        ).TotalHours
                    );

        DateTime currentTime =
            DateTime.UtcNow;

        int overdueTickets =
            allVisibleTickets.Count(ticket =>
                ticket.DueDate.HasValue
                &&
                ticket.DueDate.Value <
                    currentTime
                &&
                ticket.StatusId !=
                    TicketStatusIds.Resolved
                &&
                ticket.StatusId !=
                    TicketStatusIds.Closed
                &&
                ticket.StatusId !=
                    TicketStatusIds.Canceled
            );

        var result =
            new DashboardAnalyticsDto
            {
                Role =
                    role,

                From =
                    startDate,

                To =
                    to.Date,

                TotalTickets =
                    periodTickets.Count,

                PendingReviewTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.PendingReview
                    ),

                OpenTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.Open
                    ),

                AssignedTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.Assigned
                    ),

                InProgressTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.InProgress
                    ),

                ResolvedTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.Resolved
                    ),

                ClosedTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.Closed
                    ),

                CanceledTickets =
                    CountStatus(
                        periodTickets,
                        TicketStatusIds.Canceled
                    ),

                OverdueTickets =
                    overdueTickets,

                AverageResolutionHours =
                    Math.Round(
                        averageResolutionHours,
                        2
                    ),

                StatusBreakdown =
                    BuildStatusBreakdown(
                        periodTickets
                    ),

                PriorityBreakdown =
                    BuildPriorityBreakdown(
                        periodTickets
                    ),

                CategoryBreakdown =
                    BuildCategoryBreakdown(
                        periodTickets
                    ),

                Trend =
                    BuildTrend(
                        allVisibleTickets,
                        startDate,
                        endExclusive
                    )
            };

        await AddRoleSpecificInformationAsync(
            result,
            role
        );

        return result;
    }

    // =====================================================
    // ROLE-SPECIFIC METRICS
    // =====================================================

    private async Task
        AddRoleSpecificInformationAsync(
            DashboardAnalyticsDto result,
            string role)
    {
        if (
            role == "Admin"
            ||
            role == "Manager"
        )
        {
            result.AvailableOpenTickets =
                await _dashboardRepository
                    .GetAvailableOpenTicketCountAsync();

            result.PendingAssignmentRequests =
                await _dashboardRepository
                    .GetPendingAssignmentRequestCountAsync();

            result.ActiveAgents =
                await _dashboardRepository
                    .GetActiveAgentCountAsync();

            return;
        }

        if (
            role == "IT Support Agent"
        )
        {
            result.AvailableOpenTickets =
                await _dashboardRepository
                    .GetAvailableOpenTicketCountAsync();
        }
    }

    // =====================================================
    // STATUS COUNT
    // =====================================================

    private static int CountStatus(
        IEnumerable<Ticket> tickets,
        int statusId)
    {
        return tickets.Count(ticket =>
            ticket.StatusId ==
                statusId
        );
    }

    // =====================================================
    // STATUS BREAKDOWN
    // =====================================================

    private static List<DashboardBreakdownDto>
        BuildStatusBreakdown(
            IEnumerable<Ticket> tickets)
    {
        return tickets
            .GroupBy(ticket =>
                ticket.Status?.Name ??
                "Unknown")
            .Select(group =>
                new DashboardBreakdownDto
                {
                    Name =
                        group.Key,

                    Count =
                        group.Count()
                }
            )
            .OrderByDescending(item =>
                item.Count)
            .ToList();
    }

    // =====================================================
    // PRIORITY BREAKDOWN
    // =====================================================

    private static List<DashboardBreakdownDto>
        BuildPriorityBreakdown(
            IEnumerable<Ticket> tickets)
    {
        return tickets
            .GroupBy(ticket =>
                ticket.Priority?.Name ??
                "Unknown")
            .Select(group =>
                new DashboardBreakdownDto
                {
                    Name =
                        group.Key,

                    Count =
                        group.Count()
                }
            )
            .OrderByDescending(item =>
                item.Count)
            .ToList();
    }

    // =====================================================
    // CATEGORY BREAKDOWN
    // =====================================================

    private static List<DashboardBreakdownDto>
        BuildCategoryBreakdown(
            IEnumerable<Ticket> tickets)
    {
        return tickets
            .GroupBy(ticket =>
                ticket.Category?.Name ??
                "Unknown")
            .Select(group =>
                new DashboardBreakdownDto
                {
                    Name =
                        group.Key,

                    Count =
                        group.Count()
                }
            )
            .OrderByDescending(item =>
                item.Count)
            .ToList();
    }

    // =====================================================
    // CREATED / RESOLVED / CLOSED TREND
    // =====================================================

    private static List<DashboardTrendPointDto>
        BuildTrend(
            List<Ticket> tickets,
            DateTime startDate,
            DateTime endExclusive)
    {
        double numberOfDays =
            (
                endExclusive -
                startDate
            ).TotalDays;

        /*
         * Use daily points for periods up to
         * approximately two months.
         */
        if (numberOfDays <= 62)
        {
            return BuildDailyTrend(
                tickets,
                startDate,
                endExclusive
            );
        }

        /*
         * Use monthly points for longer periods.
         */
        return BuildMonthlyTrend(
            tickets,
            startDate,
            endExclusive
        );
    }

    private static List<DashboardTrendPointDto>
        BuildDailyTrend(
            List<Ticket> tickets,
            DateTime startDate,
            DateTime endExclusive)
    {
        var createdCounts =
            tickets
                .Where(ticket =>
                    ticket.CreatedDate >=
                        startDate
                    &&
                    ticket.CreatedDate <
                        endExclusive
                )
                .GroupBy(ticket =>
                    ticket.CreatedDate.Date
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

        var resolvedCounts =
            tickets
                .Where(ticket =>
                    ticket.ResolvedDate.HasValue
                    &&
                    ticket.ResolvedDate.Value >=
                        startDate
                    &&
                    ticket.ResolvedDate.Value <
                        endExclusive
                )
                .GroupBy(ticket =>
                    ticket.ResolvedDate!
                        .Value.Date
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

        var closedCounts =
            tickets
                .Where(ticket =>
                    ticket.ClosedDate.HasValue
                    &&
                    ticket.ClosedDate.Value >=
                        startDate
                    &&
                    ticket.ClosedDate.Value <
                        endExclusive
                )
                .GroupBy(ticket =>
                    ticket.ClosedDate!
                        .Value.Date
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

        var result =
            new List<DashboardTrendPointDto>();

        for (
            DateTime day = startDate;
            day < endExclusive;
            day = day.AddDays(1)
        )
        {
            result.Add(
                new DashboardTrendPointDto
                {
                    Period =
                        day.ToString("yyyy-MM-dd"),

                    Created =
                        createdCounts
                            .GetValueOrDefault(
                                day.Date
                            ),

                    Resolved =
                        resolvedCounts
                            .GetValueOrDefault(
                                day.Date
                            ),

                    Closed =
                        closedCounts
                            .GetValueOrDefault(
                                day.Date
                            )
                }
            );
        }

        return result;
    }

    private static List<DashboardTrendPointDto>
        BuildMonthlyTrend(
            List<Ticket> tickets,
            DateTime startDate,
            DateTime endExclusive)
    {
        var createdCounts =
            tickets
                .Where(ticket =>
                    ticket.CreatedDate >=
                        startDate
                    &&
                    ticket.CreatedDate <
                        endExclusive
                )
                .GroupBy(ticket =>
                    new DateTime(
                        ticket.CreatedDate.Year,
                        ticket.CreatedDate.Month,
                        1
                    )
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

        var resolvedCounts =
            tickets
                .Where(ticket =>
                    ticket.ResolvedDate.HasValue
                    &&
                    ticket.ResolvedDate.Value >=
                        startDate
                    &&
                    ticket.ResolvedDate.Value <
                        endExclusive
                )
                .GroupBy(ticket =>
                    new DateTime(
                        ticket.ResolvedDate!.Value.Year,
                        ticket.ResolvedDate.Value.Month,
                        1
                    )
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

        var closedCounts =
            tickets
                .Where(ticket =>
                    ticket.ClosedDate.HasValue
                    &&
                    ticket.ClosedDate.Value >=
                        startDate
                    &&
                    ticket.ClosedDate.Value <
                        endExclusive
                )
                .GroupBy(ticket =>
                    new DateTime(
                        ticket.ClosedDate!.Value.Year,
                        ticket.ClosedDate.Value.Month,
                        1
                    )
                )
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

        var result =
            new List<DashboardTrendPointDto>();

        DateTime month =
            new(
                startDate.Year,
                startDate.Month,
                1
            );

        DateTime finalMonth =
            new(
                endExclusive
                    .AddDays(-1).Year,

                endExclusive
                    .AddDays(-1).Month,

                1
            );

        while (month <= finalMonth)
        {
            result.Add(
                new DashboardTrendPointDto
                {
                    Period =
                        month.ToString("yyyy-MM"),

                    Created =
                        createdCounts
                            .GetValueOrDefault(
                                month
                            ),

                    Resolved =
                        resolvedCounts
                            .GetValueOrDefault(
                                month
                            ),

                    Closed =
                        closedCounts
                            .GetValueOrDefault(
                                month
                            )
                }
            );

            month =
                month.AddMonths(1);
        }

        return result;
    }
}