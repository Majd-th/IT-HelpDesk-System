using Microsoft.EntityFrameworkCore;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Repositories;

public class TicketAttachmentRepository
    : ITicketAttachmentRepository
{
    private readonly ApplicationDbContext _context;

    public TicketAttachmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TicketAttachment attachment)
    {
        _context.TicketAttachments.Add(attachment);

        await _context.SaveChangesAsync();
    }

    public async Task<List<TicketAttachment>> GetByTicketIdAsync(int ticketId)
    {
        return await _context.TicketAttachments
            .Where(a => a.TicketId == ticketId)
            .ToListAsync();
    }

    public async Task<TicketAttachment?> GetByIdAsync(int id)
    {
        return await _context.TicketAttachments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAsync(TicketAttachment attachment)
    {
        _context.TicketAttachments.Remove(attachment);

        await _context.SaveChangesAsync();
    }
}