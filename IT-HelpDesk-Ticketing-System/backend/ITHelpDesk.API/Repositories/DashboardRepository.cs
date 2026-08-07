using ITHelpDesk.API.Constants;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.API.Repositories;

public class DashboardRepository
    : IDashboardRepository
{
    private readonly ApplicationDbContext
        _context;

    public DashboardRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // =====================================================
    // GET TICKETS VISIBLE TO THE CURRENT ROLE
    // =====================================================

    public async Task<List<Ticket>>
        GetVisibleTicketsAsync(
            int userId,
            string role)
    {
        var query =
            _context.Tickets
                .AsNoTracking()
                .Include(ticket =>
                    ticket.Category)
                .Include(ticket =>
                    ticket.Priority)
                .Include(ticket =>
                    ticket.Status)
                .Include(ticket =>
                    ticket.CreatedByUser)
                .Include(ticket =>
                    ticket.AssignedToUser)
                .AsQueryable();

        switch (role)
        {
            case "Admin":
            case "Manager":
                /*
                 * Admin and Manager see system-wide
                 * ticket analytics.
                 */
                break;

            case "IT Support Agent":
                /*
                 * Agent dashboard contains tickets
                 * assigned to that Agent.
                 */
                query = query.Where(ticket =>
                    ticket.AssignedToUserId ==
                        userId
                );

                break;

            case "Employee":
                /*
                 * Employee dashboard contains only
                 * tickets created by that Employee.
                 *
                 * Public solved tickets belong on the
                 * Tickets page, not personal KPI cards.
                 */
                query = query.Where(ticket =>
                    ticket.CreatedByUserId ==
                        userId
                );

                break;

            default:
                query = query.Where(ticket =>
                    false
                );

                break;
        }

        return await query
            .OrderByDescending(ticket =>
                ticket.CreatedDate)
            .ToListAsync();
    }

    // =====================================================
    // CURRENT OPEN AND UNASSIGNED TICKETS
    // =====================================================

    public async Task<int>
        GetAvailableOpenTicketCountAsync()
    {
        return await _context.Tickets
            .AsNoTracking()
            .CountAsync(ticket =>
                ticket.StatusId ==
                    TicketStatusIds.Open
                &&
                ticket.AssignedToUserId ==
                    null
            );
    }

    // =====================================================
    // CURRENT PENDING AGENT REQUESTS
    // =====================================================

    public async Task<int>
        GetPendingAssignmentRequestCountAsync()
    {
        return await _context
            .TicketAssignments
            .AsNoTracking()
            .CountAsync(assignment =>
                assignment.AssignmentType ==
                    AssignmentTypes.AgentRequest
                &&
                assignment.ApprovalStatus ==
                    AssignmentApprovalStatuses.Pending
            );
    }

    // =====================================================
    // ACTIVE IT SUPPORT AGENTS
    // =====================================================

    public async Task<int>
        GetActiveAgentCountAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .CountAsync(user =>
                user.IsActive
                &&
                user.Role.Name ==
                    "IT Support Agent"
            );
    }
}