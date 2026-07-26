using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly ApplicationDbContext _context;

    public ActivityLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ActivityLog log)
    {
        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ActivityLog>> GetByTicketIdAsync(int ticketId)
    {
        return await _context.ActivityLogs
            .Include(a => a.User)
            .Where(a => a.TicketId == ticketId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }
    public async Task<List<ActivityLog>> GetActivityLogsAsync(int ticketId)
    {
        return await _context.ActivityLogs
            .Include(a => a.User)
            .Where(a => a.TicketId == ticketId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }
}