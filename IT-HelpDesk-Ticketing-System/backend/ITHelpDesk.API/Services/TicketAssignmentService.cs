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
    private readonly INotificationService
_notificationService;

    private const int MaximumActiveTicketsPerAgent = 5;

    public TicketAssignmentService(
        ITicketAssignmentRepository assignmentRepository,
        INotificationService notificationService)
    {
        _assignmentRepository = assignmentRepository;
        _notificationService = notificationService;
    }

    // =====================================================
    // MANAGER / ADMIN: DIRECTLY ASSIGN A TICKET
    // Pending Review/Open -> Assigned
    // =====================================================

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

        bool canBeAssigned =
            ticket.StatusId ==
                TicketStatusIds.PendingReview
            ||
            ticket.StatusId ==
                TicketStatusIds.Open;

        if (!canBeAssigned)
        {
            return (
                false,
                "Only Pending Review or Open tickets can be assigned."
            );
        }

        var agent =
            await _assignmentRepository
                .GetAgentByIdAsync(
                    request.AgentId);

        if (agent == null)
        {
            return (
                false,
                "The selected user is not an active IT Support Agent."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(
                    agent.Id);

        if (
            activeTicketCount >=
            MaximumActiveTicketsPerAgent
        )
        {
            return (
                false,
                "The selected agent is fully loaded."
            );
        }

        var assignment =
            new TicketAssignment
            {
                TicketId = ticket.Id,

                AssignedToUserId =
                    agent.Id,

                AssignedByUserId =
                    assignedByUserId,

                AssignmentType =
                    AssignmentTypes
                        .ManagerAssignment,

                ApprovalStatus =
                    AssignmentApprovalStatuses
                        .Approved,

                Notes =
                    NormalizeNotes(
                        request.Notes),

                AssignedDate =
                    DateTime.UtcNow,

                ReviewedDate =
                    DateTime.UtcNow,

                IsActive = true
            };

        ticket.AssignedToUserId =
            agent.Id;

        /*
         * Assignment does not mean work has started.
         * The assigned Agent must click Start Work.
         */
        ticket.StatusId =
            TicketStatusIds.Assigned;

        await _assignmentRepository
            .AddAssignmentAsync(
                assignment);

        await _assignmentRepository
            .UpdateTicketAsync(
                ticket);

        await _assignmentRepository
            .SaveChangesAsync();
        await _notificationService
.NotifyAgentTicketAssignedAsync(
    agent.Id,
    ticket.Id,
    ticket.ReferenceNumber,
    ticket.Title
);

        return (
            true,
            $"Ticket assigned to {agent.FirstName} {agent.LastName}."
        );
    }

    // =====================================================
    // MANAGER / ADMIN: REASSIGN A TICKET
    // Existing status -> Assigned for new Agent
    // =====================================================
    public async Task<(bool Success, string Message)>
        ReassignTicketAsync(
            int ticketId,
            ReassignTicketRequestDto request,
            int assignedByUserId)
    {
        var ticket =
            await _assignmentRepository
                .GetTicketByIdAsync(
                    ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket was not found."
            );
        }

        int? oldAgentId =
            ticket.AssignedToUserId;

        if (!oldAgentId.HasValue)
        {
            return (
                false,
                "This ticket is not currently assigned to an Agent."
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
                .GetAgentByIdAsync(
                    request.NewAgentId);

        if (newAgent == null)
        {
            return (
                false,
                "The selected user is not an active IT Support Agent."
            );
        }

        if (oldAgentId.Value == newAgent.Id)
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

        if (
            activeTicketCount >=
            MaximumActiveTicketsPerAgent
        )
        {
            return (
                false,
                "The selected agent is fully loaded."
            );
        }

        var currentAssignment =
            await _assignmentRepository
                .GetActiveAssignmentAsync(
                    ticketId);

        if (currentAssignment != null)
        {
            currentAssignment.IsActive =
                false;

            currentAssignment.UnassignedDate =
                DateTime.UtcNow;

            await _assignmentRepository
                .UpdateAssignmentAsync(
                    currentAssignment);
        }

        var newAssignment =
            new TicketAssignment
            {
                TicketId =
                    ticket.Id,

                AssignedToUserId =
                    newAgent.Id,

                AssignedByUserId =
                    assignedByUserId,

                AssignmentType =
                    AssignmentTypes.Reassignment,

                ApprovalStatus =
                    AssignmentApprovalStatuses
                        .Approved,

                Notes =
                    NormalizeNotes(
                        request.Notes),

                AssignedDate =
                    DateTime.UtcNow,

                ReviewedDate =
                    DateTime.UtcNow,

                IsActive =
                    true
            };

        ticket.AssignedToUserId =
            newAgent.Id;

        /*
         * The new Agent must acknowledge
         * and start the work themselves.
         */
        ticket.StatusId =
            TicketStatusIds.Assigned;

        await _assignmentRepository
            .AddAssignmentAsync(
                newAssignment);

        await _assignmentRepository
            .UpdateTicketAsync(
                ticket);

        await _assignmentRepository
            .SaveChangesAsync();

        await _notificationService
            .NotifyTicketReassignedAsync(
                oldAgentId,
                newAgent.Id,
                ticket.Id,
                ticket.ReferenceNumber,
                ticket.Title
            );

        return (
            true,
            $"Ticket reassigned to {newAgent.FirstName} {newAgent.LastName}."
        );
    }


    // =====================================================
    // AGENT: REQUEST AN OPEN TICKET
    // =====================================================

    public async Task<(bool Success, string Message)>
        RequestAssignmentAsync(
            int ticketId,
            RequestAssignmentDto request,
            int agentId)
    {
        var agent =
            await _assignmentRepository
                .GetAgentByIdAsync(
                    agentId);

        if (agent == null)
        {
            return (
                false,
                "Only active IT Support Agents can request tickets."
            );
        }

        var ticket =
            await _assignmentRepository
                .GetTicketByIdAsync(
                    ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket was not found."
            );
        }

        if (
            ticket.AssignedToUserId
                .HasValue
        )
        {
            return (
                false,
                "This ticket has already been assigned."
            );
        }

        /*
         * Agents can request only Open tickets.
         * Pending Review tickets have not yet been
         * made available by the Manager.
         */
        if (!IsOpen(ticket))
        {
            return (
                false,
                "Only Open tickets can be requested."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(
                    agentId);

        if (
            activeTicketCount >=
            MaximumActiveTicketsPerAgent
        )
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
                TicketId =
                    ticket.Id,

                AssignedToUserId =
                    agentId,

                /*
                 * During a self-request, the Agent
                 * is initially stored in both fields.
                 * AssignedByUserId will be replaced
                 * by the reviewer ID after approval.
                 */
                AssignedByUserId =
                    agentId,

                AssignmentType =
                    AssignmentTypes
                        .AgentRequest,

                ApprovalStatus =
                    AssignmentApprovalStatuses
                        .Pending,

                Notes =
                    NormalizeNotes(
                        request.Notes),

                AssignedDate =
                    DateTime.UtcNow,

                IsActive = false
            };

        await _assignmentRepository
            .AddAssignmentAsync(
                assignmentRequest);

        await _assignmentRepository
            .SaveChangesAsync();
        await _notificationService
.NotifyManagersAndAdminsAssignmentRequestAsync(
    ticket.Id,
    ticket.ReferenceNumber,
    ticket.Title,
    agent.FirstName + " " + agent.LastName
);

        return (
            true,
            "Assignment request submitted for Manager approval."
        );
    }

    // =====================================================
    // MANAGER / ADMIN: APPROVE OR REJECT AGENT REQUEST
    // =====================================================

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

        bool isPendingAgentRequest =
            assignment.AssignmentType ==
                AssignmentTypes.AgentRequest
            &&
            assignment.ApprovalStatus ==
                AssignmentApprovalStatuses
                    .Pending;

        if (!isPendingAgentRequest)
        {
            return (
                false,
                "This assignment request has already been reviewed or is not an Agent request."
            );
        }

        var ticket =
            assignment.Ticket;

        if (ticket == null)
        {
            return (
                false,
                "The related ticket was not found."
            );
        }

        // =============================================
        // REJECT REQUEST
        // =============================================

        if (!request.Approved)
        {
            assignment.ApprovalStatus =
                AssignmentApprovalStatuses
                    .Rejected;

            assignment.ReviewedDate =
                DateTime.UtcNow;
            assignment.AssignedByUserId =
                reviewerUserId;
            assignment.IsActive =
                false;

            if (
                !string.IsNullOrWhiteSpace(
                    request.Notes)
            )
            {
                assignment.Notes =
                    CombineNotes(
                        assignment.Notes,
                        request.Notes);
            }

            await _assignmentRepository
                .UpdateAssignmentAsync(
                    assignment);

            await _assignmentRepository
                .SaveChangesAsync();
            await _notificationService
.CreateNotificationAsync(
    assignment.AssignedToUserId,
    ticket.Id,
    "Assignment Request Rejected",
    $"Your request for {ticket.ReferenceNumber} - {ticket.Title} was rejected.",
    "AssignmentRejected"
);

            return (
                true,
                "Assignment request rejected."
            );
        }

        // =============================================
        // APPROVE REQUEST
        // =============================================

        if (
            ticket.AssignedToUserId
                .HasValue
        )
        {
            return (
                false,
                "This ticket was assigned to another Agent before the request was reviewed."
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
                    assignment
                        .AssignedToUserId);

        if (agent == null)
        {
            return (
                false,
                "The requesting Agent is no longer active."
            );
        }

        int activeTicketCount =
            await _assignmentRepository
                .GetAgentActiveTicketCountAsync(
                    agent.Id);

        if (
            activeTicketCount >=
            MaximumActiveTicketsPerAgent
        )
        {
            return (
                false,
                "The requesting Agent is now fully loaded."
            );
        }

        assignment.ApprovalStatus =
            AssignmentApprovalStatuses
                .Approved;

        assignment.ReviewedDate =
            DateTime.UtcNow;
        assignment.AssignedByUserId =
            reviewerUserId;
        assignment.IsActive =
            true;

        /*
         * After approval, AssignedByUserId records
         * the Manager/Admin who approved the request.
         */
        assignment.AssignedByUserId =
            reviewerUserId;

        if (
            !string.IsNullOrWhiteSpace(
                request.Notes)
        )
        {
            assignment.Notes =
                CombineNotes(
                    assignment.Notes,
                    request.Notes);
        }

        ticket.AssignedToUserId =
            assignment.AssignedToUserId;

        ticket.StatusId =
            TicketStatusIds.Assigned;

        await _assignmentRepository
            .UpdateAssignmentAsync(
                assignment);

        await _assignmentRepository
            .UpdateTicketAsync(
                ticket);

        await _assignmentRepository
            .SaveChangesAsync();
        await _notificationService
.NotifyAgentAssignmentDecisionAsync(
    assignment.AssignedToUserId,
    ticket.Id,
    ticket.ReferenceNumber,
    ticket.Title,
    request.Approved
);

        return (
            true,
            $"Assignment request approved for {agent.FirstName} {agent.LastName}."
        );
    }

    // =====================================================
    // MANAGER / ADMIN: AGENT WORKLOAD
    // =====================================================

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

            result.Add(
                new AgentWorkloadDto
                {
                    AgentId =
                        agent.Id,

                    FullName =
                        $"{agent.FirstName} {agent.LastName}",

                    Email =
                        agent.Email,

                    ActiveTicketCount =
                        activeTicketCount,

                    IsFullyLoaded =
                        activeTicketCount >=
                        MaximumActiveTicketsPerAgent
                }
            );
        }

        return result
            .OrderBy(agent =>
                agent.ActiveTicketCount)
            .ThenBy(agent =>
                agent.FullName)
            .ToList();
    }

    // =====================================================
    // MANAGER / ADMIN: PENDING REQUESTS
    // =====================================================

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

    // =====================================================
    // MANAGER / ADMIN: ASSIGNMENT HISTORY
    // =====================================================

    public async Task<List<TicketAssignmentResponseDto>>
        GetAssignmentHistoryAsync(
            int ticketId)
    {
        var assignments =
            await _assignmentRepository
                .GetTicketAssignmentHistoryAsync(
                    ticketId);

        return assignments
            .Select(MapAssignment)
            .ToList();
    }

    // =====================================================
    // AGENT OR MANAGER: AVAILABLE / UNASSIGNED TICKETS
    //
    // agentId > 0:
    //     Agent view, Open only
    //
    // agentId <= 0:
    //     Manager view, Pending Review + Open
    // =====================================================

    public async Task<List<AvailableTicketDto>>
        GetAvailableTicketsAsync(
            int agentId)
    {
        var tickets =
            await _assignmentRepository
                .GetAvailableTicketsAsync(
                    agentId);

        var result =
            new List<AvailableTicketDto>();

        foreach (var ticket in tickets)
        {
            bool hasPendingRequest =
                false;

            /*
             * A Manager passes zero, so do not search
             * for a request belonging to user ID zero.
             */
            if (agentId > 0)
            {
                var pendingRequest =
                    await _assignmentRepository
                        .GetPendingRequestAsync(
                            ticket.Id,
                            agentId);

                hasPendingRequest =
                    pendingRequest != null;
            }

            result.Add(
                MapTicket(
                    ticket,
                    hasPendingRequest)
            );
        }

        return result;
    }

    // =====================================================
    // AGENT: CURRENT ASSIGNED TICKETS
    // =====================================================

    public async Task<List<AvailableTicketDto>>
        GetAgentTicketsAsync(
            int agentId)
    {
        var tickets =
            await _assignmentRepository
                .GetAgentTicketsAsync(
                    agentId);

        return tickets
            .Select(ticket =>
                MapTicket(
                    ticket,
                    false))
            .ToList();
    }

    // =====================================================
    // AGENT: RESOLVED / CLOSED HISTORY
    // =====================================================

    public async Task<List<AvailableTicketDto>>
        GetAgentHistoryAsync(
            int agentId)
    {
        var tickets =
            await _assignmentRepository
                .GetAgentHistoryAsync(
                    agentId);

        return tickets
            .Select(ticket =>
                MapTicket(
                    ticket,
                    false))
            .ToList();
    }

    // =====================================================
    // DTO MAPPING: ASSIGNMENT
    // =====================================================

    private static TicketAssignmentResponseDto
        MapAssignment(
            TicketAssignment assignment)
    {
        return new TicketAssignmentResponseDto
        {
            Id =
                assignment.Id,

            TicketId =
                assignment.TicketId,

            TicketReference =
                assignment.Ticket?
                    .ReferenceNumber ?? "",

            TicketTitle =
                assignment.Ticket?
                    .Title ?? "",

            AssignedToUserId =
                assignment
                    .AssignedToUserId,

            AssignedToUser =
                assignment.AssignedToUser ==
                    null
                    ? ""
                    : $"{assignment.AssignedToUser.FirstName} " +
                      $"{assignment.AssignedToUser.LastName}",

            AssignedByUserId =
                assignment
                    .AssignedByUserId,

            AssignedByUser =
                assignment.AssignedByUser ==
                    null
                    ? ""
                    : $"{assignment.AssignedByUser.FirstName} " +
                      $"{assignment.AssignedByUser.LastName}",

            AssignmentType =
                assignment.AssignmentType,

            ApprovalStatus =
                assignment.ApprovalStatus,

            Notes =
                assignment.Notes,

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

    // =====================================================
    // DTO MAPPING: TICKET
    // =====================================================

    private static AvailableTicketDto
        MapTicket(
            Ticket ticket,
            bool hasPendingRequest)
    {
        return new AvailableTicketDto
        {
            Id =
                ticket.Id,

            ReferenceNumber =
                ticket.ReferenceNumber,

            Title =
                ticket.Title,

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

    // =====================================================
    // STATUS HELPERS
    // =====================================================

    private static bool IsOpen(
        Ticket ticket)
    {
        return ticket.StatusId ==
            TicketStatusIds.Open;
    }

    private static bool IsFinished(
        Ticket ticket)
    {
        return
            ticket.StatusId ==
                TicketStatusIds.Resolved
            ||
            ticket.StatusId ==
                TicketStatusIds.Closed
            ||
            ticket.StatusId ==
                TicketStatusIds.Canceled;
    }

    // =====================================================
    // NOTES HELPERS
    // =====================================================

    private static string?
        NormalizeNotes(
            string? notes)
    {
        return string.IsNullOrWhiteSpace(
            notes)
            ? null
            : notes.Trim();
    }

    private static string?
        CombineNotes(
            string? originalNotes,
            string? reviewNotes)
    {
        string? original =
            NormalizeNotes(
                originalNotes);

        string? review =
            NormalizeNotes(
                reviewNotes);

        if (original == null)
        {
            return review;
        }

        if (review == null)
        {
            return original;
        }

        return
            $"{original}\nManager review: {review}";
    }

    public async Task<List<TicketSelectionDto>>
    GetReassignableTicketsAsync()
    {
        var tickets =
            await _assignmentRepository
                .GetReassignableTicketsAsync();

        return tickets
            .Select(ticket =>
                new TicketSelectionDto
                {
                    Id = ticket.Id,

                    ReferenceNumber =
                        ticket.ReferenceNumber,

                    Title =
                        ticket.Title,

                    Status =
                        ticket.Status?.Name ?? "",

                    AssignedToUserId =
                        ticket.AssignedToUserId,

                    AssignedTo =
                        ticket.AssignedToUser == null
                            ? null
                            : $"{ticket.AssignedToUser.FirstName} " +
                              $"{ticket.AssignedToUser.LastName}"
                })
            .ToList();
    }
    public async Task<List<TicketSelectionDto>>
        GetHistoryTicketsAsync()
    {
        var tickets =
            await _assignmentRepository
                .GetHistoryTicketsAsync();

        return tickets
            .Select(ticket =>
                new TicketSelectionDto
                {
                    Id = ticket.Id,

                    ReferenceNumber =
                        ticket.ReferenceNumber,

                    Title =
                        ticket.Title,

                    Status =
                        ticket.Status?.Name ?? "",

                    AssignedToUserId =
                        ticket.AssignedToUserId,

                    AssignedTo =
                        ticket.AssignedToUser == null
                            ? null
                            : $"{ticket.AssignedToUser.FirstName} " +
                              $"{ticket.AssignedToUser.LastName}"
                })
            .ToList();
    }
}