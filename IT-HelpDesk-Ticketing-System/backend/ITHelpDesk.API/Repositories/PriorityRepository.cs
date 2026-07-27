using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Repositories;

public class PriorityRepository : IPriorityRepository
{
    private readonly ApplicationDbContext _context;

    public PriorityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Priority>> GetAllAsync()
    {
        return await _context.Priorities.ToListAsync();
    }

    public async Task<Priority?> GetByIdAsync(int id)
    {
        return await _context.Priorities.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Priority priority)
    {
        await _context.Priorities.AddAsync(priority);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Priority priority)
    {
        _context.Priorities.Update(priority);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Priority priority)
    {
        _context.Priorities.Remove(priority);
        await _context.SaveChangesAsync();
    }
}