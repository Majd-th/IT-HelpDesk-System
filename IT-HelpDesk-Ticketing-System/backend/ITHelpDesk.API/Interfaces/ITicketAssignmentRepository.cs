using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketAssignmentRepository
{
    Task<Ticket?> GetTicketByIdAsync(
        int ticketId);

    Task<User?> GetAgentByIdAsync(
        int agentId);

    Task<List<User>> GetActiveAgentsAsync();

    Task<int> GetAgentActiveTicketCountAsync(
        int agentId);

    Task<TicketAssignment?> GetActiveAssignmentAsync(
        int ticketId);

    Task<TicketAssignment?> GetAssignmentByIdAsync(
        int assignmentId);

    Task<TicketAssignment?> GetPendingRequestAsync(
        int ticketId,
        int agentId);

    Task<List<TicketAssignment>>
        GetPendingRequestsAsync();

    Task<List<TicketAssignment>>
        GetTicketAssignmentHistoryAsync(
            int ticketId);

    Task<List<Ticket>> GetAvailableTicketsAsync(
        int? agentId = null);

    Task<List<Ticket>> GetAgentTicketsAsync(
        int agentId);

    Task<List<Ticket>> GetAgentHistoryAsync(
        int agentId);

    Task AddAssignmentAsync(
        TicketAssignment assignment);

    Task UpdateAssignmentAsync(
        TicketAssignment assignment);

    Task UpdateTicketAsync(
        Ticket ticket);

    Task SaveChangesAsync();
}