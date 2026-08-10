using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

using ITHelpDesk.API.Hubs;
using Microsoft.AspNetCore.SignalR;
namespace ITHelpDesk.API.Services;

public class NotificationService
    : INotificationService
{
    private readonly INotificationRepository
        _notificationRepository;
    private readonly IHubContext<NotificationHub>
        _notificationHubContext;

    private readonly ILogger<NotificationService>
        _logger;
    public NotificationService(
        INotificationRepository
            notificationRepository,
        IHubContext<NotificationHub>
            notificationHubContext,
        ILogger<NotificationService>
            logger)
    {
        _notificationRepository =
            notificationRepository;
        _notificationHubContext =
            notificationHubContext;
        _logger =
            logger;
    }

    // =====================================================
    // GET CURRENT USER NOTIFICATIONS
    // =====================================================

    public async Task<List<NotificationResponseDto>>
        GetUserNotificationsAsync(
            int userId)
    {
        var notifications =
            await _notificationRepository
                .GetUserNotificationsAsync(
                    userId
                );

        return notifications
            .Select(notification =>
                new NotificationResponseDto
                {
                    Id =
                        notification.Id,

                    TicketId =
                        notification.TicketId,

                    TicketReference =
                        notification.Ticket
                            ?.ReferenceNumber,

                    Title =
                        notification.Title,

                    Message =
                        notification.Message,

                    Type =
                        notification.Type,

                    IsRead =
                        notification.IsRead,

                    CreatedDate =
                        notification.CreatedDate,

                    ReadDate =
                        notification.ReadDate
                }
            )
            .ToList();
    }

    // =====================================================
    // UNREAD COUNT
    // =====================================================

    public async Task<int>
        GetUnreadCountAsync(
            int userId)
    {
        return await _notificationRepository
            .GetUnreadCountAsync(
                userId
            );
    }

    // =====================================================
    // MARK ONE AS READ
    // =====================================================

    public async Task<(bool Success, string Message)>
        MarkAsReadAsync(
            int notificationId,
            int userId)
    {
        var notification =
            await _notificationRepository
                .GetByIdAsync(
                    notificationId
                );

        if (notification == null)
        {
            return (
                false,
                "Notification not found."
            );
        }

        /*
         * A user may only modify
         * their own notification.
         */
        if (
            notification.UserId !=
            userId
        )
        {
            return (
                false,
                "You cannot modify this notification."
            );
        }

        if (notification.IsRead)
        {
            return (
                true,
                "Notification is already marked as read."
            );
        }

        notification.IsRead =
            true;

        notification.ReadDate =
            DateTime.UtcNow;

        await _notificationRepository
            .SaveChangesAsync();

        return (
            true,
            "Notification marked as read."
        );
    }

    // =====================================================
    // MARK ALL AS READ
    // =====================================================

    public async Task<(bool Success, string Message)>
        MarkAllAsReadAsync(
            int userId)
    {
        var notifications =
            await _notificationRepository
                .GetUserNotificationsAsync(
                    userId
                );

        var unread =
            notifications
                .Where(notification =>
                    !notification.IsRead)
                .ToList();

        if (unread.Count == 0)
        {
            return (
                true,
                "There are no unread notifications."
            );
        }

        DateTime readDate =
            DateTime.UtcNow;

        /*
         * Get tracked notification entities
         * individually because the list query
         * uses AsNoTracking.
         */
        foreach (
            var notification in unread
        )
        {
            var trackedNotification =
                await _notificationRepository
                    .GetByIdAsync(
                        notification.Id
                    );

            if (
                trackedNotification ==
                null
            )
            {
                continue;
            }

            trackedNotification.IsRead =
                true;

            trackedNotification.ReadDate =
                readDate;
        }

        await _notificationRepository
            .SaveChangesAsync();

        return (
            true,
            "All notifications marked as read."
        );
    }

    // =====================================================
    // CREATE ONE NOTIFICATION
    // =====================================================

    public async Task
        CreateNotificationAsync(
            int userId,
            int? ticketId,
            string title,
            string message,
            string type)
    {
        var notification =
            new Notification
            {
                UserId =
                    userId,

                TicketId =
                    ticketId,

                Title =
                    title.Trim(),

                Message =
                    message.Trim(),

                Type =
                    type.Trim(),

                IsRead =
                    false,

                CreatedDate =
                    DateTime.UtcNow
            };

        await _notificationRepository
            .AddAsync(notification);

        await _notificationRepository
            .SaveChangesAsync(); await PushRealtimeNotificationAsync(
    userId);

    }

    // =====================================================
    // CREATE FOR MULTIPLE USERS
    // =====================================================

    public async Task
        CreateNotificationsAsync(
            IEnumerable<int> userIds,
            int? ticketId,
            string title,
            string message,
            string type)
    {
        /*
         * Distinct prevents duplicate
         * notifications for the same user.
         */
        var ids =
            userIds
                .Distinct()
                .ToList();

        if (ids.Count == 0)
        {
            return;
        }

        DateTime createdDate =
            DateTime.UtcNow;

        var notifications =
            ids.Select(userId =>
                new Notification
                {
                    UserId =
                        userId,

                    TicketId =
                        ticketId,

                    Title =
                        title.Trim(),

                    Message =
                        message.Trim(),

                    Type =
                        type.Trim(),

                    IsRead =
                        false,

                    CreatedDate =
                        createdDate
                }
            )
            .ToList();

        await _notificationRepository
            .AddRangeAsync(
                notifications
            );

        await _notificationRepository
            .SaveChangesAsync();
        foreach (var userId in ids)
        {
            await PushRealtimeNotificationAsync(
                userId
            );
        }
    }
    public async Task
    NotifyManagersAndAdminsAsync(
        int ticketId,
        string ticketReference,
        string title)
    {
        var recipientIds =
            await _notificationRepository
                .GetActiveUserIdsByRolesAsync(
                    "Manager",
                    "Admin"
                );

        if (recipientIds.Count == 0)
        {
            return;
        }

        await CreateNotificationsAsync(
            recipientIds,
            ticketId,
            "New Ticket Created",
            $"{ticketReference} - {title}",
            "TicketCreated"
        );
    }
    public async Task
    NotifyAgentsTicketPublishedAsync(
        int ticketId,
        string ticketReference,
        string ticketTitle)
    {
        var agentIds =
            await _notificationRepository
                .GetActiveUserIdsByRolesAsync(
                    "IT Support Agent"
                );

        if (agentIds.Count == 0)
        {
            return;
        }

        await CreateNotificationsAsync(
            agentIds,
            ticketId,
            "New Ticket Available",
            $"{ticketReference} - {ticketTitle} is now available to request.",
            "TicketPublished"
        );
    }
    public async Task
    NotifyAgentTicketAssignedAsync(
        int agentId,
        int ticketId,
        string ticketReference,
        string ticketTitle)
    {
        await CreateNotificationAsync(
            agentId,
            ticketId,
            "Ticket Assigned to You",
            $"{ticketReference} - {ticketTitle} has been assigned to you.",
            "TicketAssigned"
        );
    }
    public async Task
    NotifyManagersAndAdminsAssignmentRequestAsync(
        int ticketId,
        string ticketReference,
        string ticketTitle,
        string agentName)
    {
        var recipientIds =
            await _notificationRepository
                .GetActiveUserIdsByRolesAsync(
                    "Manager",
                    "Admin"
                );

        if (recipientIds.Count == 0)
        {
            return;
        }

        await CreateNotificationsAsync(
            recipientIds,
            ticketId,
            "New Ticket Assignment Request",
            $"{agentName} requested {ticketReference} - {ticketTitle}.",
            "AssignmentRequested"
        );
    }


    public async Task
    NotifyAgentAssignmentDecisionAsync(
        int agentId,
        int ticketId,
        string ticketReference,
        string ticketTitle,
        bool approved)
    {
        if (approved)
        {
            await CreateNotificationAsync(
                agentId,
                ticketId,
                "Assignment Request Approved",
                $"Your request for {ticketReference} - {ticketTitle} was approved.",
                "AssignmentApproved"
            );
        }
        else
        {
            await CreateNotificationAsync(
                agentId,
                ticketId,
                "Assignment Request Rejected",
                $"Your request for {ticketReference} - {ticketTitle} was rejected.",
                "AssignmentRejected"
            );
        }
    }
    public async Task
    NotifyTicketCreatorWorkStartedAsync(
        int creatorUserId,
        int ticketId,
        string ticketReference,
        string ticketTitle)
    {
        await CreateNotificationAsync(
            creatorUserId,
            ticketId,
            "Work Started on Your Ticket",
            $"{ticketReference} - {ticketTitle}: An IT Support Agent has started working on your ticket.",
            "WorkStarted"
        );
    }

    public async Task NotifyTicketResolvedAsync(
    int creatorUserId,
    int ticketId,
    string ticketReference,
    string ticketTitle)
    {
        // Notify the person who created the ticket
        await CreateNotificationAsync(
            creatorUserId,
            ticketId,
            "Your Ticket Was Resolved",
            $"{ticketReference} - {ticketTitle} has been resolved.",
            "TicketResolved"
        );

        // Notify Managers and Admins
        var managerAdminIds =
            await _notificationRepository
                .GetActiveUserIdsByRolesAsync(
                    "Manager",
                    "Admin"
                );

        /*
         * If the creator is an Admin,
         * do not send them the same event twice.
         */
        managerAdminIds =
            managerAdminIds
                .Where(userId =>
                    userId != creatorUserId)
                .ToList();

        if (managerAdminIds.Count == 0)
        {
            return;
        }

        await CreateNotificationsAsync(
            managerAdminIds,
            ticketId,
            "Ticket Resolved",
            $"{ticketReference} - {ticketTitle} has been resolved by IT Support.",
            "TicketResolved"
        );
    }
    public async Task NotifyTicketClosedAsync(
    int creatorUserId,
    int? assignedAgentId,
    int closedByUserId,
    int ticketId,
    string ticketReference,
    string ticketTitle)
    {
        if (creatorUserId != closedByUserId)
        {
            await CreateNotificationAsync(
                creatorUserId,
                ticketId,
                "Your Ticket Was Closed",
                $"{ticketReference} - {ticketTitle} has been closed.",
                "TicketClosed"
            );
        }

        if (
            assignedAgentId.HasValue &&
            assignedAgentId.Value != closedByUserId
        )
        {
            await CreateNotificationAsync(
                assignedAgentId.Value,
                ticketId,
                "Ticket Closed",
                $"{ticketReference} - {ticketTitle} has been closed.",
                "TicketClosed"
            );
        }
    }
    public async Task NotifyTicketReassignedAsync(
    int? oldAgentId,
    int newAgentId,
    int ticketId,
    string ticketReference,
    string ticketTitle)
    {
        // Notify previous Agent
        if (
            oldAgentId.HasValue &&
            oldAgentId.Value != newAgentId
        )
        {
            await CreateNotificationAsync(
                oldAgentId.Value,
                ticketId,
                "Ticket Reassigned",
                $"{ticketReference} - {ticketTitle} has been reassigned away from you.",
                "TicketReassigned"
            );
        }

        // Notify new Agent
        await CreateNotificationAsync(
            newAgentId,
            ticketId,
            "Ticket Reassigned to You",
            $"{ticketReference} - {ticketTitle} has been reassigned to you.",
            "TicketReassigned"
        );
    }
    private async Task
    PushRealtimeNotificationAsync(
        int userId)
    {
        try
        {
            await _notificationHubContext
                .Clients
                .User(
                    userId.ToString()
                )
                .SendAsync(
                    "NotificationReceived"
                );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Could not push realtime notification to user {UserId}.",
                userId
            );
        }
    }
}