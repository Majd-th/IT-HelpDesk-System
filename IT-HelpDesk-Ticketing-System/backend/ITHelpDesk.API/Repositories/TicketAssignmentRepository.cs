using ITHelpDesk.API.Constants;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.API.Repositories;

public class TicketAssignmentRepository
    : ITicketAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public TicketAssignmentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetTicketByIdAsync(
        int ticketId)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t =>
                t.Id == ticketId);
    }

    public async Task<User?> GetAgentByIdAsync(
        int agentId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Id == agentId &&
                u.IsActive &&
                u.Role.Name == "IT Support Agent");
    }

    public async Task<List<User>>
        GetActiveAgentsAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Where(u =>
                u.IsActive &&
                u.Role.Name == "IT Support Agent")
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();
    }

    public async Task<int>
        GetAgentActiveTicketCountAsync(
            int agentId)
    {
        return await _context.Tickets
            .CountAsync(t =>
                t.AssignedToUserId == agentId &&
                (
                    t.Status.Name == "Open" ||
                    t.Status.Name == "In Progress" ||
                    t.Status.Name == "Pending"
                ));
    }

    public async Task<TicketAssignment?>
        GetActiveAssignmentAsync(int ticketId)
    {
        return await _context.TicketAssignments
            .Include(a => a.Ticket)
            .Include(a => a.AssignedToUser)
            .Include(a => a.AssignedByUser)
            .FirstOrDefaultAsync(a =>
                a.TicketId == ticketId &&
                a.IsActive &&
                a.ApprovalStatus ==
                    AssignmentApprovalStatuses.Approved);
    }

    public async Task<TicketAssignment?>
        GetAssignmentByIdAsync(int assignmentId)
    {
        return await _context.TicketAssignments
            .Include(a => a.Ticket)
                .ThenInclude(t => t.Status)
            .Include(a => a.AssignedToUser)
            .Include(a => a.AssignedByUser)
            .FirstOrDefaultAsync(a =>
                a.Id == assignmentId);
    }

    public async Task<TicketAssignment?>
        GetPendingRequestAsync(
            int ticketId,
            int agentId)
    {
        return await _context.TicketAssignments
            .FirstOrDefaultAsync(a =>
                a.TicketId == ticketId &&
                a.AssignedToUserId == agentId &&
                a.AssignmentType ==
                    AssignmentTypes.AgentRequest &&
                a.ApprovalStatus ==
                    AssignmentApprovalStatuses.Pending);
    }

    public async Task<List<TicketAssignment>>
        GetPendingRequestsAsync()
    {
        return await _context.TicketAssignments
            .Include(a => a.Ticket)
                .ThenInclude(t => t.Category)
            .Include(a => a.Ticket)
                .ThenInclude(t => t.Priority)
            .Include(a => a.Ticket)
                .ThenInclude(t => t.Status)
            .Include(a => a.AssignedToUser)
            .Include(a => a.AssignedByUser)
            .Where(a =>
                a.AssignmentType ==
                    AssignmentTypes.AgentRequest &&
                a.ApprovalStatus ==
                    AssignmentApprovalStatuses.Pending)
            .OrderBy(a => a.AssignedDate)
            .ToListAsync();
    }

    public async Task<List<TicketAssignment>>
        GetTicketAssignmentHistoryAsync(
            int ticketId)
    {
        return await _context.TicketAssignments
            .Include(a => a.AssignedToUser)
            .Include(a => a.AssignedByUser)
            .Where(a => a.TicketId == ticketId)
            .OrderByDescending(a =>
                a.AssignedDate)
            .ToListAsync();
    }

    public async Task<List<Ticket>>
        GetAvailableTicketsAsync(
            int? agentId = null)
    {
        var query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .Where(t =>
                t.AssignedToUserId == null &&
                t.Status.Name == "Open");

        /*
         * The agentId parameter is kept for later use,
         * when we mark tickets already requested by
         * the logged-in agent.
         */
        return await query
            .OrderByDescending(t =>
                t.Priority.DisplayOrder)
            .ThenBy(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Ticket>>
        GetAgentTicketsAsync(int agentId)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .Where(t =>
                t.AssignedToUserId == agentId &&
                (
                    t.Status.Name == "Open" ||
                    t.Status.Name == "In Progress" ||
                    t.Status.Name == "Pending"
                ))
            .OrderByDescending(t =>
                t.Priority.DisplayOrder)
            .ThenBy(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<Ticket>>
        GetAgentHistoryAsync(int agentId)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .Where(t =>
                t.AssignedToUserId == agentId &&
                (
                    t.Status.Name == "Resolved" ||
                    t.Status.Name == "Closed"
                ))
            .OrderByDescending(t =>
                t.ResolvedDate ??
                t.ClosedDate ??
                t.CreatedDate)
            .ToListAsync();
    }

    public async Task AddAssignmentAsync(
        TicketAssignment assignment)
    {
        await _context.TicketAssignments
            .AddAsync(assignment);
    }

    public Task UpdateAssignmentAsync(
        TicketAssignment assignment)
    {
        _context.TicketAssignments.Update(
            assignment);

        return Task.CompletedTask;
    }

    public Task UpdateTicketAsync(
        Ticket ticket)
    {
        _context.Tickets.Update(ticket);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}