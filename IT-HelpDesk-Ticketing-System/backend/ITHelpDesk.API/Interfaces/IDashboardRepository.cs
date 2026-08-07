

using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IDashboardRepository
{
    Task<List<Ticket>>
        GetVisibleTicketsAsync(
            int userId,
            string role
        );

    Task<int>
        GetAvailableOpenTicketCountAsync();

    Task<int>
        GetPendingAssignmentRequestCountAsync();

    Task<int>
        GetActiveAgentCountAsync();
}