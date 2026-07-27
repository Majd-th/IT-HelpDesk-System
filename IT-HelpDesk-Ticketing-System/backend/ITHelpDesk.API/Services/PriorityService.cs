using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Services;

public class PriorityService : IPriorityService
{
    private readonly IPriorityRepository _repository;

    public PriorityService(IPriorityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Priority>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Priority?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Priority> CreateAsync(Priority priority)
    {
        await _repository.AddAsync(priority);
        return priority;
    }

    public async Task<bool> UpdateAsync(int id, Priority priority)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
            return false;

        existing.Name = priority.Name;
        existing.DisplayOrder = priority.DisplayOrder;

        await _repository.UpdateAsync(existing);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
            return false;

        await _repository.DeleteAsync(existing);

        return true;
    }
}