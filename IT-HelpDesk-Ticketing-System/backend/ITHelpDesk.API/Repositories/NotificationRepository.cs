using ITHelpDesk.API.Data;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.API.Repositories;

public class NotificationRepository
    : INotificationRepository
{
    private readonly ApplicationDbContext
        _context;

    public NotificationRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>>
        GetUserNotificationsAsync(
            int userId)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Include(notification =>
                notification.Ticket)
            .Where(notification =>
                notification.UserId == userId)
            .OrderByDescending(notification =>
                notification.CreatedDate)
            .ToListAsync();
    }

    public async Task<Notification?>
        GetByIdAsync(
            int notificationId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(notification =>
                notification.Id ==
                    notificationId);
    }

    public async Task<int>
        GetUnreadCountAsync(
            int userId)
    {
        return await _context.Notifications
            .AsNoTracking()
            .CountAsync(notification =>
                notification.UserId == userId
                &&
                !notification.IsRead);
    }

    public async Task AddAsync(
        Notification notification)
    {
        await _context.Notifications
            .AddAsync(notification);
    }

    public async Task AddRangeAsync(
        IEnumerable<Notification> notifications)
    {
        await _context.Notifications
            .AddRangeAsync(notifications);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<List<int>>
    GetActiveUserIdsByRolesAsync(
        params string[] roles)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(user =>
                user.IsActive &&
                roles.Contains(user.Role.Name)
            )
            .Select(user =>
                user.Id)
            .ToListAsync();
    }
}