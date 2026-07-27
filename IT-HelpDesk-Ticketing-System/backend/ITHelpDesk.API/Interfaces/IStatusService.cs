using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IStatusService
{
    Task<List<Status>> GetAllAsync();
    Task<Status?> GetByIdAsync(int id);
    Task<Status> CreateAsync(Status status);
    Task<bool> UpdateAsync(int id, Status status);
    Task<bool> DeleteAsync(int id);
}