using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Services;

public class StatusService : IStatusService
{
    private readonly IStatusRepository _repository;

    public StatusService(IStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Status>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Status?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Status> CreateAsync(Status status)
    {
        await _repository.AddAsync(status);
        return status;
    }

    public async Task<bool> UpdateAsync(int id, Status status)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing == null)
            return false;

        existing.Name = status.Name;
        existing.DisplayOrder = status.DisplayOrder;

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