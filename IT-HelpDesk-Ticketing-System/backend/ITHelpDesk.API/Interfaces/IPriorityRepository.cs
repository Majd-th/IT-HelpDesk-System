using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IPriorityRepository
{
    Task<List<Priority>> GetAllAsync();
    Task<Priority?> GetByIdAsync(int id);
    Task AddAsync(Priority priority);
    Task UpdateAsync(Priority priority);
    Task DeleteAsync(Priority priority);
}