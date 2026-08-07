using ITHelpDesk.API.DTOs;

namespace ITHelpDesk.API.Interfaces;

public interface IDashboardService
{
    Task<DashboardAnalyticsDto>
        GetAnalyticsAsync(
            int userId,
            string role,
            DateTime from,
            DateTime to
        );
}