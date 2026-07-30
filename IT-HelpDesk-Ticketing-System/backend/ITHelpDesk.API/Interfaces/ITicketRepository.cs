using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();

    Task<Ticket?> GetByIdAsync(int id);

    Task AddAsync(Ticket ticket);

    Task UpdateAsync(Ticket ticket);

    Task DeleteAsync(Ticket ticket);

    Task SaveChangesAsync();

    Task<List<Ticket>> FilterTicketsAsync(
        TicketFilterDto filter);
}