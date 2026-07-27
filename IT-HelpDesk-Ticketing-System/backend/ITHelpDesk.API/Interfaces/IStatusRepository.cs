using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IStatusRepository
{
    Task<List<Status>> GetAllAsync();
    Task<Status?> GetByIdAsync(int id);
    Task AddAsync(Status status);
    Task UpdateAsync(Status status);
    Task DeleteAsync(Status status);
}