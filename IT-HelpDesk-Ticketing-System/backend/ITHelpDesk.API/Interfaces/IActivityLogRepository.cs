using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface IActivityLogRepository
{
    Task AddAsync(ActivityLog log);

    Task<List<ActivityLog>> GetByTicketIdAsync(int ticketId);
    Task<List<ActivityLog>> GetActivityLogsAsync(int userId);
}