using System.Security.Claims;
using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketAssignmentController : ControllerBase
{
    private readonly ITicketAssignmentService
        _assignmentService;

    public TicketAssignmentController(
        ITicketAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    // =====================================================
    // MANAGER / ADMIN: VIEW AGENT WORKLOAD
    // GET /api/TicketAssignment/agents/workload
    // =====================================================

    [HttpGet("agents/workload")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetAgentWorkloads()
    {
        var workloads =
            await _assignmentService
                .GetAgentWorkloadsAsync();

        return Ok(workloads);
    }

    // =====================================================
    // MANAGER / ADMIN: ASSIGN TICKET DIRECTLY
    // POST /api/TicketAssignment/{ticketId}/assign
    // =====================================================

    [HttpPost("{ticketId}/assign")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> AssignTicket(
        int ticketId,
        AssignTicketRequestDto request)
    {
        int userId = GetCurrentUserId();

        var result =
            await _assignmentService
                .AssignTicketAsync(
                    ticketId,
                    request,
                    userId);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // =====================================================
    // MANAGER / ADMIN: REASSIGN TICKET
    // PUT /api/TicketAssignment/{ticketId}/reassign
    // =====================================================

    [HttpPut("{ticketId}/reassign")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ReassignTicket(
        int ticketId,
        ReassignTicketRequestDto request)
    {
        int userId = GetCurrentUserId();

        var result =
            await _assignmentService
                .ReassignTicketAsync(
                    ticketId,
                    request,
                    userId);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // =====================================================
    // MANAGER / ADMIN: VIEW PENDING AGENT REQUESTS
    // GET /api/TicketAssignment/requests
    // =====================================================

    [HttpGet("requests")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var requests =
            await _assignmentService
                .GetPendingRequestsAsync();

        return Ok(requests);
    }

    // =====================================================
    // MANAGER / ADMIN: APPROVE OR REJECT REQUEST
    // PUT /api/TicketAssignment/requests/{assignmentId}/review
    // =====================================================

    [HttpPut("requests/{assignmentId}/review")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ReviewRequest(
        int assignmentId,
        ReviewAssignmentRequestDto request)
    {
        int reviewerUserId =
            GetCurrentUserId();

        var result =
            await _assignmentService
                .ReviewRequestAsync(
                    assignmentId,
                    request,
                    reviewerUserId);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // =====================================================
    // MANAGER / ADMIN: VIEW ASSIGNMENT HISTORY
    // GET /api/TicketAssignment/{ticketId}/history
    // =====================================================

    [HttpGet("{ticketId}/history")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetAssignmentHistory(
        int ticketId)
    {
        var history =
            await _assignmentService
                .GetAssignmentHistoryAsync(
                    ticketId);

        return Ok(history);
    }

    // =====================================================
    // AGENT / ADMIN: VIEW AVAILABLE OPEN TICKETS
    // GET /api/TicketAssignment/available
    // =====================================================

    [HttpGet("available")]
    [Authorize(Roles = "IT Support Agent,Admin")]
    public async Task<IActionResult> GetAvailableTickets()
    {
        int userId = GetCurrentUserId();

        var tickets =
            await _assignmentService
                .GetAvailableTicketsAsync(
                    userId);

        return Ok(tickets);
    }

    // =====================================================
    // AGENT: REQUEST AN AVAILABLE TICKET
    // POST /api/TicketAssignment/{ticketId}/request
    // =====================================================

    [HttpPost("{ticketId}/request")]
    [Authorize(Roles = "IT Support Agent")]
    public async Task<IActionResult> RequestTicket(
        int ticketId,
        RequestAssignmentDto request)
    {
        int agentId = GetCurrentUserId();

        var result =
            await _assignmentService
                .RequestAssignmentAsync(
                    ticketId,
                    request,
                    agentId);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message
        });
    }

    // =====================================================
    // AGENT / ADMIN: VIEW CURRENT ASSIGNED TICKETS
    // GET /api/TicketAssignment/my-tickets
    // =====================================================

    [HttpGet("my-tickets")]
    [Authorize(Roles = "IT Support Agent,Admin")]
    public async Task<IActionResult> GetMyTickets()
    {
        int userId = GetCurrentUserId();

        var tickets =
            await _assignmentService
                .GetAgentTicketsAsync(
                    userId);

        return Ok(tickets);
    }

    // =====================================================
    // AGENT / ADMIN: VIEW RESOLVED/CLOSED HISTORY
    // GET /api/TicketAssignment/my-history
    // =====================================================

    [HttpGet("my-history")]
    [Authorize(Roles = "IT Support Agent,Admin")]
    public async Task<IActionResult> GetMyHistory()
    {
        int userId = GetCurrentUserId();

        var tickets =
            await _assignmentService
                .GetAgentHistoryAsync(
                    userId);

        return Ok(tickets);
    }

    // =====================================================
    // HELPER: READ USER ID FROM JWT
    // =====================================================

    private int GetCurrentUserId()
    {
        string? userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (
            string.IsNullOrWhiteSpace(userIdValue) ||
            !int.TryParse(
                userIdValue,
                out int userId)
        )
        {
            throw new UnauthorizedAccessException(
                "The authenticated user could not be identified."
            );
        }

        return userId;
    }
    // =====================================================
    // MANAGER / ADMIN: VIEW UNASSIGNED OPEN TICKETS
    // GET /api/TicketAssignment/unassigned
    // =====================================================

    [HttpGet("unassigned")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetUnassignedTickets()
    {
        /*
         * Manager is not requesting the ticket, so zero is
         * passed only to disable the agent-request indicator.
         */
        var tickets =
            await _assignmentService
                .GetAvailableTicketsAsync(0);

        return Ok(tickets);
    }
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("reassignable-tickets")]
    public async Task<IActionResult>
    GetReassignableTickets()
    {
        var tickets =
            await _assignmentService
                .GetReassignableTicketsAsync();

        return Ok(tickets);
    }


    [Authorize(Roles = "Manager,Admin")]
    [HttpGet("history-tickets")]
    public async Task<IActionResult>
    GetHistoryTickets()
    {
        var tickets =
            await _assignmentService
                .GetHistoryTicketsAsync();

        return Ok(tickets);
    }
}