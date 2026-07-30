using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.API.Repositories;

public class TicketAttachmentRepository
    : ITicketAttachmentRepository
{
    private readonly ApplicationDbContext _context;

    public TicketAttachmentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        TicketAttachment attachment)
    {
        await _context.TicketAttachments
            .AddAsync(attachment);

        await _context.SaveChangesAsync();
    }

    public async Task<List<TicketAttachment>>
        GetByTicketIdAsync(int ticketId)
    {
        return await _context.TicketAttachments
            .Where(a => a.TicketId == ticketId)
            .OrderByDescending(a => a.UploadedDate)
            .ToListAsync();
    }

    public async Task<TicketAttachment?>
        GetByIdAsync(int attachmentId)
    {
        return await _context.TicketAttachments
            .FirstOrDefaultAsync(
                a => a.Id == attachmentId);
    }

    public async Task DeleteAsync(
        TicketAttachment attachment)
    {
        _context.TicketAttachments
            .Remove(attachment);

        await _context.SaveChangesAsync();
    }
}