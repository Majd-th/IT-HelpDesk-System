using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>>
        GetUserNotificationsAsync(
            int userId
        );

    Task<Notification?>
        GetByIdAsync(
            int notificationId
        );

    Task<int>
        GetUnreadCountAsync(
            int userId
        );

    Task AddAsync(
        Notification notification
    );

    Task AddRangeAsync(
        IEnumerable<Notification> notifications
    );
    Task<List<int>> GetActiveUserIdsByRolesAsync(
        params string[] roles
    );
    Task SaveChangesAsync();
}