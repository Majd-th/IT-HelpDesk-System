using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using System.Diagnostics;

using ITHelpDesk.API.DTOs;
namespace ITHelpDesk.API.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;


    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Ticket>> GetAllAsync()
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<List<ActivityLog>> GetActivityLogsAsync(int ticketId)
    {
        return await _context.ActivityLogs
            .Where(a => a.TicketId == ticketId)
            .ToListAsync();
    }
    public async Task<List<Ticket>> FilterTicketsAsync(TicketFilterDto filter)
    {
        var query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
        }

        if (filter.PriorityId.HasValue)
        {
            query = query.Where(t => t.PriorityId == filter.PriorityId.Value);
        }

        if (filter.StatusId.HasValue)
        {
            query = query.Where(t => t.StatusId == filter.StatusId.Value);
        }

        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(t => t.CreatedDate >= filter.CreatedAfter.Value);
        }

        if (filter.CreatedBefore.HasValue)
        {
            query = query.Where(t => t.CreatedDate <= filter.CreatedBefore.Value);
        }

        return await query.ToListAsync();
    }
}
