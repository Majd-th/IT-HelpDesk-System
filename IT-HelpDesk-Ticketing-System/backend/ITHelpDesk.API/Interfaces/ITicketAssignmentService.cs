using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Models;
namespace ITHelpDesk.API.Interfaces;

public interface ITicketAssignmentService
{
    Task<(bool Success, string Message)> AssignTicketAsync(
        int ticketId,
        AssignTicketRequestDto request,
        int assignedByUserId);

    Task<(bool Success, string Message)> ReassignTicketAsync(
        int ticketId,
        ReassignTicketRequestDto request,
        int assignedByUserId);

    Task<(bool Success, string Message)> RequestAssignmentAsync(
        int ticketId,
        RequestAssignmentDto request,
        int agentId);

    Task<(bool Success, string Message)> ReviewRequestAsync(
        int assignmentId,
        ReviewAssignmentRequestDto request,
        int reviewerUserId);

    Task<List<AgentWorkloadDto>> GetAgentWorkloadsAsync();

    Task<List<TicketAssignmentResponseDto>>
        GetPendingRequestsAsync();

    Task<List<TicketAssignmentResponseDto>>
        GetAssignmentHistoryAsync(int ticketId);

    Task<List<AvailableTicketDto>>
        GetAvailableTicketsAsync(int agentId);

    Task<List<AvailableTicketDto>>
        GetAgentTicketsAsync(int agentId);

    Task<List<AvailableTicketDto>>
        GetAgentHistoryAsync(int agentId);


    Task<List<TicketSelectionDto>>
        GetReassignableTicketsAsync();

    Task<List<TicketSelectionDto>>
        GetHistoryTicketsAsync();
}