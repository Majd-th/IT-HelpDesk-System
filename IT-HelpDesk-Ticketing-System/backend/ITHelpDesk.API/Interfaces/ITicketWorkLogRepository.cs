using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketWorkLogRepository
{
    Task<TicketWorkLog?> GetActiveWorkLogAsync(
        int ticketId,
        int agentId);

    Task AddAsync(TicketWorkLog workLog);

    Task SaveChangesAsync();
}