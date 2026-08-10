using ITHelpDesk.API.DTOs;

namespace ITHelpDesk.API.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponseDto>>
        GetUserNotificationsAsync(
            int userId
        );

    Task<int>
        GetUnreadCountAsync(
            int userId
        );

    Task<(bool Success, string Message)>
        MarkAsReadAsync(
            int notificationId,
            int userId
        );

    Task<(bool Success, string Message)>
        MarkAllAsReadAsync(
            int userId
        );

    Task CreateNotificationAsync(
        int userId,
        int? ticketId,
        string title,
        string message,
        string type
    );

    Task CreateNotificationsAsync(
        IEnumerable<int> userIds,
        int? ticketId,
        string title,
        string message,
        string type
    );
    Task NotifyManagersAndAdminsAsync(
    int ticketId,
    string ticketReference,
    string title
);
    Task NotifyAgentsTicketPublishedAsync(
        int ticketId,
        string ticketReference,
        string ticketTitle
    );
    Task NotifyAgentTicketAssignedAsync(
    int agentId,
    int ticketId,
    string ticketReference,
    string ticketTitle
); Task NotifyManagersAndAdminsAssignmentRequestAsync(
    int ticketId,
    string ticketReference,
    string ticketTitle,
    string agentName
); Task NotifyAgentAssignmentDecisionAsync(
    int agentId,
    int ticketId,
    string ticketReference,
    string ticketTitle,
    bool approved
); Task NotifyTicketCreatorWorkStartedAsync(
    int creatorUserId,
    int ticketId,
    string ticketReference,
    string ticketTitle
);
    Task NotifyTicketResolvedAsync(
        int creatorUserId,
        int ticketId,
        string ticketReference,
        string ticketTitle
    );
    Task NotifyTicketClosedAsync(
        int creatorUserId,
        int? assignedAgentId,
        int closedByUserId,
        int ticketId,
        string ticketReference,
        string ticketTitle
    );
    Task NotifyTicketReassignedAsync(
    int? oldAgentId,
    int newAgentId,
    int ticketId,
    string ticketReference,
    string ticketTitle
);
}