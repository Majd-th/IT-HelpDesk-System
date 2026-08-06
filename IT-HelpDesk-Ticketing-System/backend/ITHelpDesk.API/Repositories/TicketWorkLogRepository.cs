using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.API.Repositories;

public class TicketWorkLogRepository
    : ITicketWorkLogRepository
{
    private readonly ApplicationDbContext _context;

    public TicketWorkLogRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketWorkLog?>
        GetActiveWorkLogAsync(
            int ticketId,
            int agentId)
    {
        return await _context.TicketWorkLogs
            .FirstOrDefaultAsync(workLog =>
                workLog.TicketId == ticketId &&
                workLog.AgentId == agentId &&
                workLog.EndTime == null);
    }

    public async Task AddAsync(
        TicketWorkLog workLog)
    {
        await _context.TicketWorkLogs.AddAsync(
            workLog);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}