using ITHelpDesk.API.Constants;
using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Services;

public class TicketAssignmentService
    : ITicketAssignmentService
{
    private readonly ITicketAssignmentRepository
        _assignmentRepository;

    private const int MaximumActiveTicketsPerAgent = 5;

    public TicketAssignmentService(
        ITicketAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<(bool Success, string Message)>
        AssignTicketAsync(
            int ticketId,
            AssignTicketRequestDto request,
            int assignedByUserId)
    {
        var ticket =
            await _assignmentRepository
                .GetTicketByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket was not found."
            );
        }

        if (ticket.AssignedToUserId.HasValue)
        {
            return (
                false,
                "This ticket is already assigned. Use the reassign operation instead."
            );
        }

        if (IsFinished(ticket))
        {
            return (
                false,
                "Resolved or closed tickets cannot be assigned."
            );
        }

        var agent =
            await _assignmentRepository
                .GetAgentByIdAsync(request.AgentId);

        if (agent == null)
        {
            return (
                false,
                "The selected user is not an active IT Support Agent."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(agent.Id);

        if (activeTicketCount >= MaximumActiveTicketsPerAgent)
        {
            return (
                false,
                "The selected agent is fully loaded."
            );
        }

        var assignment = new TicketAssignment
        {
            TicketId = ticket.Id,

            AssignedToUserId = agent.Id,

            AssignedByUserId = assignedByUserId,

            AssignmentType =
                AssignmentTypes.ManagerAssignment,

            ApprovalStatus =
                AssignmentApprovalStatuses.Approved,

            Notes = NormalizeNotes(request.Notes),

            AssignedDate = DateTime.UtcNow,

            ReviewedDate = DateTime.UtcNow,

            IsActive = true
        };

        ticket.AssignedToUserId = agent.Id;

        /*
         * A directly assigned ticket becomes In Progress.
         * Your seeded status ID 2 is "In Progress".
         */
        ticket.StatusId = 2;

        await _assignmentRepository
            .AddAssignmentAsync(assignment);

        await _assignmentRepository
            .UpdateTicketAsync(ticket);

        await _assignmentRepository
            .SaveChangesAsync();

        return (
            true,
            $"Ticket assigned to {agent.FirstName} {agent.LastName}."
        );
    }

    public async Task<(bool Success, string Message)>
        ReassignTicketAsync(
            int ticketId,
            ReassignTicketRequestDto request,
            int assignedByUserId)
    {
        var ticket =
            await _assignmentRepository
                .GetTicketByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket was not found."
            );
        }

        if (IsFinished(ticket))
        {
            return (
                false,
                "Resolved or closed tickets cannot be reassigned."
            );
        }

        var newAgent =
            await _assignmentRepository
                .GetAgentByIdAsync(request.NewAgentId);

        if (newAgent == null)
        {
            return (
                false,
                "The selected user is not an active IT Support Agent."
            );
        }

        if (ticket.AssignedToUserId == newAgent.Id)
        {
            return (
                false,
                "The ticket is already assigned to this agent."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(
                    newAgent.Id);

        if (activeTicketCount >= MaximumActiveTicketsPerAgent)
        {
            return (
                false,
                "The selected agent is fully loaded."
            );
        }

        var currentAssignment =
            await _assignmentRepository
                .GetActiveAssignmentAsync(ticketId);

        if (currentAssignment != null)
        {
            currentAssignment.IsActive = false;
            currentAssignment.UnassignedDate =
                DateTime.UtcNow;

            await _assignmentRepository
                .UpdateAssignmentAsync(
                    currentAssignment);
        }

        var newAssignment = new TicketAssignment
        {
            TicketId = ticket.Id,

            AssignedToUserId = newAgent.Id,

            AssignedByUserId = assignedByUserId,

            AssignmentType =
                AssignmentTypes.Reassignment,

            ApprovalStatus =
                AssignmentApprovalStatuses.Approved,

            Notes = NormalizeNotes(request.Notes),

            AssignedDate = DateTime.UtcNow,

            ReviewedDate = DateTime.UtcNow,

            IsActive = true
        };

        ticket.AssignedToUserId = newAgent.Id;

        if (IsOpen(ticket))
        {
            ticket.StatusId = 2;
        }

        await _assignmentRepository
            .AddAssignmentAsync(newAssignment);

        await _assignmentRepository
            .UpdateTicketAsync(ticket);

        await _assignmentRepository
            .SaveChangesAsync();

        return (
            true,
            $"Ticket reassigned to {newAgent.FirstName} {newAgent.LastName}."
        );
    }

    public async Task<(bool Success, string Message)>
        RequestAssignmentAsync(
            int ticketId,
            RequestAssignmentDto request,
            int agentId)
    {
        var agent =
            await _assignmentRepository
                .GetAgentByIdAsync(agentId);

        if (agent == null)
        {
            return (
                false,
                "Only active IT Support Agents can request tickets."
            );
        }

        var ticket =
            await _assignmentRepository
                .GetTicketByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket was not found."
            );
        }

        if (ticket.AssignedToUserId.HasValue)
        {
            return (
                false,
                "This ticket has already been assigned."
            );
        }

        if (!IsOpen(ticket))
        {
            return (
                false,
                "Only Open tickets can be requested."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(agentId);

        if (activeTicketCount >= MaximumActiveTicketsPerAgent)
        {
            return (
                false,
                "You are fully loaded and cannot request another ticket."
            );
        }

        var existingRequest =
            await _assignmentRepository
                .GetPendingRequestAsync(
                    ticketId,
                    agentId);

        if (existingRequest != null)
        {
            return (
                false,
                "You already have a pending request for this ticket."
            );
        }

        var assignmentRequest =
            new TicketAssignment
            {
                TicketId = ticket.Id,

                AssignedToUserId = agentId,

                /*
                 * For a self-request, the requesting agent is
                 * initially stored in both fields.
                 */
                AssignedByUserId = agentId,

                AssignmentType =
                    AssignmentTypes.AgentRequest,

                ApprovalStatus =
                    AssignmentApprovalStatuses.Pending,

                Notes = NormalizeNotes(request.Notes),

                AssignedDate = DateTime.UtcNow,

                IsActive = false
            };

        await _assignmentRepository
            .AddAssignmentAsync(assignmentRequest);

        await _assignmentRepository
            .SaveChangesAsync();

        return (
            true,
            "Assignment request submitted for manager approval."
        );
    }

    public async Task<(bool Success, string Message)>
        ReviewRequestAsync(
            int assignmentId,
            ReviewAssignmentRequestDto request,
            int reviewerUserId)
    {
        var assignment =
            await _assignmentRepository
                .GetAssignmentByIdAsync(
                    assignmentId);

        if (assignment == null)
        {
            return (
                false,
                "Assignment request was not found."
            );
        }

        if (
            assignment.AssignmentType !=
                AssignmentTypes.AgentRequest ||
            assignment.ApprovalStatus !=
                AssignmentApprovalStatuses.Pending
        )
        {
            return (
                false,
                "This assignment request has already been reviewed or is not an agent request."
            );
        }

        var ticket = assignment.Ticket;

        if (ticket == null)
        {
            return (
                false,
                "The related ticket was not found."
            );
        }

        if (!request.Approved)
        {
            assignment.ApprovalStatus =
                AssignmentApprovalStatuses.Rejected;

            assignment.ReviewedDate =
                DateTime.UtcNow;

            assignment.IsActive = false;

            if (!string.IsNullOrWhiteSpace(
                    request.Notes))
            {
                assignment.Notes =
                    CombineNotes(
                        assignment.Notes,
                        request.Notes);
            }

            await _assignmentRepository
                .UpdateAssignmentAsync(assignment);

            await _assignmentRepository
                .SaveChangesAsync();

            return (
                true,
                "Assignment request rejected."
            );
        }

        if (ticket.AssignedToUserId.HasValue)
        {
            return (
                false,
                "This ticket was assigned to another agent before the request was reviewed."
            );
        }

        if (!IsOpen(ticket))
        {
            return (
                false,
                "Only Open tickets can be approved for assignment."
            );
        }

        var agent =
            await _assignmentRepository
                .GetAgentByIdAsync(
                    assignment.AssignedToUserId);

        if (agent == null)
        {
            return (
                false,
                "The requesting agent is no longer active."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(
                    agent.Id);

        if (activeTicketCount >=
            MaximumActiveTicketsPerAgent)
        {
            return (
                false,
                "The requesting agent is now fully loaded."
            );
        }

        assignment.ApprovalStatus =
            AssignmentApprovalStatuses.Approved;

        assignment.ReviewedDate =
            DateTime.UtcNow;

        assignment.IsActive = true;

        /*
         * The reviewer becomes the user who approved
         * the assignment.
         */
        assignment.AssignedByUserId =
            reviewerUserId;

        if (!string.IsNullOrWhiteSpace(
                request.Notes))
        {
            assignment.Notes =
                CombineNotes(
                    assignment.Notes,
                    request.Notes);
        }

        ticket.AssignedToUserId =
            assignment.AssignedToUserId;

        ticket.StatusId = 2;

        await _assignmentRepository
            .UpdateAssignmentAsync(assignment);

        await _assignmentRepository
            .UpdateTicketAsync(ticket);

        await _assignmentRepository
            .SaveChangesAsync();

        return (
            true,
            $"Assignment request approved for {agent.FirstName} {agent.LastName}."
        );
    }

    public async Task<List<AgentWorkloadDto>>
        GetAgentWorkloadsAsync()
    {
        var agents =
            await _assignmentRepository
                .GetActiveAgentsAsync();

        var result =
            new List<AgentWorkloadDto>();

        foreach (var agent in agents)
        {
            int activeTicketCount =
                await _assignmentRepository
                    .GetAgentActiveTicketCountAsync(
                        agent.Id);

            result.Add(new AgentWorkloadDto
            {
                AgentId = agent.Id,

                FullName =
                    $"{agent.FirstName} {agent.LastName}",

                Email = agent.Email,

                ActiveTicketCount =
                    activeTicketCount,

                IsFullyLoaded =
                    activeTicketCount >=
                    MaximumActiveTicketsPerAgent
            });
        }

        return result
            .OrderBy(a => a.ActiveTicketCount)
            .ThenBy(a => a.FullName)
            .ToList();
    }

    public async Task<List<TicketAssignmentResponseDto>>
        GetPendingRequestsAsync()
    {
        var assignments =
            await _assignmentRepository
                .GetPendingRequestsAsync();

        return assignments
            .Select(MapAssignment)
            .ToList();
    }

    public async Task<List<TicketAssignmentResponseDto>>
        GetAssignmentHistoryAsync(int ticketId)
    {
        var assignments =
            await _assignmentRepository
                .GetTicketAssignmentHistoryAsync(
                    ticketId);

        return assignments
            .Select(MapAssignment)
            .ToList();
    }

    public async Task<List<AvailableTicketDto>>
        GetAvailableTicketsAsync(int agentId)
    {
        var tickets =
            await _assignmentRepository
                .GetAvailableTicketsAsync(
                    agentId);

        var result =
            new List<AvailableTicketDto>();

        foreach (var ticket in tickets)
        {
            var pendingRequest =
                await _assignmentRepository
                    .GetPendingRequestAsync(
                        ticket.Id,
                        agentId);

            result.Add(
                MapTicket(
                    ticket,
                    pendingRequest != null));
        }

        return result;
    }

    public async Task<List<AvailableTicketDto>>
        GetAgentTicketsAsync(int agentId)
    {
        var tickets =
            await _assignmentRepository
                .GetAgentTicketsAsync(agentId);

        return tickets
            .Select(ticket =>
                MapTicket(ticket, false))
            .ToList();
    }

    public async Task<List<AvailableTicketDto>>
        GetAgentHistoryAsync(int agentId)
    {
        var tickets =
            await _assignmentRepository
                .GetAgentHistoryAsync(agentId);

        return tickets
            .Select(ticket =>
                MapTicket(ticket, false))
            .ToList();
    }

    private static TicketAssignmentResponseDto
        MapAssignment(
            TicketAssignment assignment)
    {
        return new TicketAssignmentResponseDto
        {
            Id = assignment.Id,

            TicketId = assignment.TicketId,

            TicketReference =
                assignment.Ticket?
                    .ReferenceNumber ?? "",

            TicketTitle =
                assignment.Ticket?
                    .Title ?? "",

            AssignedToUserId =
                assignment.AssignedToUserId,

            AssignedToUser =
                assignment.AssignedToUser == null
                    ? ""
                    : $"{assignment.AssignedToUser.FirstName} " +
                      $"{assignment.AssignedToUser.LastName}",

            AssignedByUserId =
                assignment.AssignedByUserId,

            AssignedByUser =
                assignment.AssignedByUser == null
                    ? ""
                    : $"{assignment.AssignedByUser.FirstName} " +
                      $"{assignment.AssignedByUser.LastName}",

            AssignmentType =
                assignment.AssignmentType,

            ApprovalStatus =
                assignment.ApprovalStatus,

            Notes = assignment.Notes,

            AssignedDate =
                assignment.AssignedDate,

            ReviewedDate =
                assignment.ReviewedDate,

            UnassignedDate =
                assignment.UnassignedDate,

            IsActive =
                assignment.IsActive
        };
    }

    private static AvailableTicketDto
        MapTicket(
            Ticket ticket,
            bool hasPendingRequest)
    {
        return new AvailableTicketDto
        {
            Id = ticket.Id,

            ReferenceNumber =
                ticket.ReferenceNumber,

            Title = ticket.Title,

            Description =
                ticket.Description,

            Category =
                ticket.Category?.Name ?? "",

            Priority =
                ticket.Priority?.Name ?? "",

            Status =
                ticket.Status?.Name ?? "",

            CreatedBy =
                ticket.CreatedByUser == null
                    ? ""
                    : $"{ticket.CreatedByUser.FirstName} " +
                      $"{ticket.CreatedByUser.LastName}",

            CreatedDate =
                ticket.CreatedDate,

            HasPendingRequest =
                hasPendingRequest
        };
    }

    private static bool IsOpen(Ticket ticket)
    {
        return string.Equals(
            ticket.Status?.Name,
            "Open",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsFinished(Ticket ticket)
    {
        return string.Equals(
                   ticket.Status?.Name,
                   "Resolved",
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   ticket.Status?.Name,
                   "Closed",
                   StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeNotes(
        string? notes)
    {
        return string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
    }

    private static string? CombineNotes(
        string? originalNotes,
        string? reviewNotes)
    {
        string? original =
            NormalizeNotes(originalNotes);

        string? review =
            NormalizeNotes(reviewNotes);

        if (original == null)
            return review;

        if (review == null)
            return original;

        return $"{original}\nManager review: {review}";
    }
}