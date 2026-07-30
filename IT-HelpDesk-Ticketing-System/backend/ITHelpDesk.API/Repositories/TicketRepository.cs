using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

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
            .OrderByDescending(t => t.CreatedDate)
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
    }

    public Task UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);

        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Ticket>> FilterTicketsAsync(
        TicketFilterDto filter)
    {
        IQueryable<Ticket> query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.CreatedByUser);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim();

            query = query.Where(t =>
                EF.Functions.Like(
                    t.Title,
                    $"%{search}%"
                ) ||
                EF.Functions.Like(
                    t.Description,
                    $"%{search}%"
                ) ||
                EF.Functions.Like(
                    t.ReferenceNumber,
                    $"%{search}%"
                ) ||
                EF.Functions.Like(
                    t.Category.Name,
                    $"%{search}%"
                ) ||
                EF.Functions.Like(
                    t.Priority.Name,
                    $"%{search}%"
                ) ||
                EF.Functions.Like(
                    t.Status.Name,
                    $"%{search}%"
                )
            );
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(t =>
                t.CategoryId == filter.CategoryId.Value);
        }

        if (filter.PriorityId.HasValue)
        {
            query = query.Where(t =>
                t.PriorityId == filter.PriorityId.Value);
        }

        if (filter.StatusId.HasValue)
        {
            query = query.Where(t =>
                t.StatusId == filter.StatusId.Value);
        }

        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(t =>
                t.CreatedDate >= filter.CreatedAfter.Value);
        }

        if (filter.CreatedBefore.HasValue)
        {
            DateTime createdBefore =
                filter.CreatedBefore.Value;

            /*
             * A date input normally produces midnight.
             * Adding one day allows the selected end date
             * to include the entire day.
             */
            if (createdBefore.TimeOfDay == TimeSpan.Zero)
            {
                createdBefore =
                    createdBefore.Date.AddDays(1);

                query = query.Where(t =>
                    t.CreatedDate < createdBefore);
            }
            else
            {
                query = query.Where(t =>
                    t.CreatedDate <= createdBefore);
            }
        }

        return await query
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }
}