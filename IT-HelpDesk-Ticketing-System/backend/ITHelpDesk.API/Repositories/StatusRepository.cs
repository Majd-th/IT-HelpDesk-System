using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Repositories;

public class StatusRepository : IStatusRepository
{
    private readonly ApplicationDbContext _context;

    public StatusRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Status>> GetAllAsync()
    {
        return await _context.Statuses.ToListAsync();
    }

    public async Task<Status?> GetByIdAsync(int id)
    {
        return await _context.Statuses.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(Status status)
    {
        await _context.Statuses.AddAsync(status);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Status status)
    {
        _context.Statuses.Update(status);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Status status)
    {
        _context.Statuses.Remove(status);
        await _context.SaveChangesAsync();
    }
}