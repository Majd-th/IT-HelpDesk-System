using ITHelpDesk.API.Models;
using ITHelpDesk.API.DTOs;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();

    Task<Ticket?> GetByIdAsync(int id);

    Task AddAsync(Ticket ticket);

    Task UpdateAsync(Ticket ticket);

    Task DeleteAsync(Ticket ticket);

    Task SaveChangesAsync();
    Task<List<ActivityLog>> GetActivityLogsAsync(int ticketId);
    Task<List<Ticket>> FilterTicketsAsync(TicketFilterDto filter);
}