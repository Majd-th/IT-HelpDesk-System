using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IPriorityService
{
    Task<List<Priority>> GetAllAsync();
    Task<Priority?> GetByIdAsync(int id);
    Task<Priority> CreateAsync(Priority priority);
    Task<bool> UpdateAsync(int id, Priority priority);
    Task<bool> DeleteAsync(int id);
}