using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketAttachmentRepository
{
    Task AddAsync(TicketAttachment attachment);

    Task<List<TicketAttachment>> GetByTicketIdAsync(int ticketId);

    Task<TicketAttachment?> GetByIdAsync(int id);

    Task DeleteAsync(TicketAttachment attachment);
}