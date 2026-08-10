using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using ITHelpDesk.API.Constants;

namespace ITHelpDesk.API.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository
    _ticketRepository;
    private readonly INotificationService
        _notificationService;
    private readonly ITicketWorkLogRepository
        _workLogRepository;

    private readonly IActivityLogRepository
        _activityLogRepository;
    public TicketService(
        ITicketRepository ticketRepository,
        ITicketWorkLogRepository workLogRepository,
        IActivityLogRepository activityLogRepository,
        INotificationService notificationService)
    {
        _ticketRepository = ticketRepository;
        _workLogRepository = workLogRepository;
        _activityLogRepository = activityLogRepository;
        _notificationService = notificationService;
    }
    public async Task<TicketResponseDto> CreateTicketAsync(
        CreateTicketRequestDto request,
        int userId)
    {
        var ticket = new Ticket
        {
            ReferenceNumber = $"TKT-{DateTime.UtcNow:yyyyMMddHHmmss}",

            Title = request.Title,
            Description = request.Description,

            CategoryId = request.CategoryId,
            PriorityId = request.PriorityId,

            CreatedByUserId = userId,

            StatusId = TicketStatusIds.PendingReview,
            CreatedDate = DateTime.UtcNow
        };

        await _ticketRepository.AddAsync(ticket);
        await _ticketRepository.SaveChangesAsync();
        await _notificationService
    .NotifyManagersAndAdminsAsync(
        ticket.Id,
        ticket.ReferenceNumber,
        ticket.Title
    );
        await _activityLogRepository.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Created ticket"
        });

        ticket = await _ticketRepository.GetByIdAsync(ticket.Id);

        if (ticket == null)
            throw new Exception("Ticket is null");

        if (ticket.Category == null)
            throw new Exception("Category is null");

        if (ticket.Priority == null)
            throw new Exception("Priority is null");

        if (ticket.Status == null)
            throw new Exception("Status is null");

        if (ticket.CreatedByUser == null)
            throw new Exception("CreatedByUser is null");
        return new TicketResponseDto
        {
            Id = ticket.Id,

            Title = ticket.Title,
            Description = ticket.Description,

            CategoryId = ticket.CategoryId,
            PriorityId = ticket.PriorityId,
            StatusId = ticket.StatusId,

            Category = ticket.Category.Name,
            Priority = ticket.Priority.Name,
            Status = ticket.Status.Name,
            CreatedByUserId = ticket.CreatedByUserId,


            ReferenceNumber = ticket.ReferenceNumber,

            AssignedToUserId =
    ticket.AssignedToUserId,

            AssignedTo =
    ticket.AssignedToUser == null
        ? null
        : $"{ticket.AssignedToUser.FirstName} " +
          $"{ticket.AssignedToUser.LastName}",

            CreatedBy = $"{ticket.CreatedByUser.FirstName} {ticket.CreatedByUser.LastName}",

            CreatedDate = ticket.CreatedDate,
            DueDate = ticket.DueDate,
            ResolvedDate = ticket.ResolvedDate,
            ClosedDate = ticket.ClosedDate,

            Solution = ticket.Solution
        };

    }
    public async Task<List<TicketResponseDto>> GetAllTicketsAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();

        return tickets.Select(ticket => new TicketResponseDto
        {
            Id = ticket.Id,

            Title = ticket.Title,
            Description = ticket.Description,
            CreatedByUserId = ticket.CreatedByUserId,

            CategoryId = ticket.CategoryId,
            PriorityId = ticket.PriorityId,
            StatusId = ticket.StatusId,

            Category = ticket.Category.Name,
            Priority = ticket.Priority.Name,
            Status = ticket.Status.Name,

            ReferenceNumber = ticket.ReferenceNumber,
            AssignedToUserId =
    ticket.AssignedToUserId,

            AssignedTo =
    ticket.AssignedToUser == null
        ? null
        : $"{ticket.AssignedToUser.FirstName} " +
          $"{ticket.AssignedToUser.LastName}",

            CreatedBy = $"{ticket.CreatedByUser.FirstName} {ticket.CreatedByUser.LastName}",

            CreatedDate = ticket.CreatedDate,
            DueDate = ticket.DueDate,
            ResolvedDate = ticket.ResolvedDate,
            ClosedDate = ticket.ClosedDate,

            Solution = ticket.Solution
        }).ToList();
    }


    public async Task<TicketResponseDto?> GetTicketByIdAsync(int id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
            return null;
        return new TicketResponseDto
        {
            Id = ticket.Id,

            Title = ticket.Title,
            Description = ticket.Description,
            CreatedByUserId = ticket.CreatedByUserId,
            CategoryId = ticket.CategoryId,
            PriorityId = ticket.PriorityId,
            StatusId = ticket.StatusId,

            Category = ticket.Category.Name,
            Priority = ticket.Priority.Name,
            Status = ticket.Status.Name,


            ReferenceNumber = ticket.ReferenceNumber,
            AssignedToUserId =
    ticket.AssignedToUserId,

            AssignedTo =
    ticket.AssignedToUser == null
        ? null
        : $"{ticket.AssignedToUser.FirstName} " +
          $"{ticket.AssignedToUser.LastName}",


            CreatedBy = $"{ticket.CreatedByUser.FirstName} {ticket.CreatedByUser.LastName}",

            CreatedDate = ticket.CreatedDate,
            DueDate = ticket.DueDate,
            ResolvedDate = ticket.ResolvedDate,
            ClosedDate = ticket.ClosedDate,

            Solution = ticket.Solution
        };
    }
    public async Task<(bool Success, string Message)>
    UpdateTicketAsync(
        int id,
        UpdateTicketRequestDto request,
        int userId,
        string role
    )
    {
        var ticket =
            await _ticketRepository
                .GetByIdAsync(id);

        if (ticket == null)
        {
            return (
                false,
                "Ticket not found."
            );
        }

        if (
            ticket.StatusId ==
                TicketStatusIds.Closed
            ||
            ticket.StatusId ==
                TicketStatusIds.Canceled
        )
        {
            return (
                false,
                "Closed or canceled tickets cannot be edited."
            );
        }

        if (role == "Employee")
        {
            if (
                ticket.CreatedByUserId != userId
            )
            {
                return (
                    false,
                    "You can edit only your own tickets."
                );
            }

            if (
                ticket.StatusId !=
                    TicketStatusIds.PendingReview
            )
            {
                return (
                    false,
                    "The ticket can no longer be edited because it has already been reviewed."
                );
            }
        }
        else if (
            role == "Manager" ||
            role == "Admin"
        )
        {
            if (
                ticket.StatusId !=
                    TicketStatusIds.PendingReview
            )
            {
                return (
                    false,
                    "Ticket details can only be edited during Pending Review."
                );
            }
        }
        else
        {
            return (
                false,
                "IT Support Agents cannot edit the ticket's original details."
            );
        }

        string oldTitle = ticket.Title;
        string oldDescription =
            ticket.Description;

        int oldCategoryId =
            ticket.CategoryId;

        int oldPriorityId =
            ticket.PriorityId;

        ticket.Title =
            request.Title.Trim();

        ticket.Description =
            request.Description.Trim();

        ticket.CategoryId =
            request.CategoryId;

        ticket.PriorityId =
            request.PriorityId;

        /*
         * Do not update:
         *
         * ticket.StatusId
         * ticket.Solution
         *
         * Status and solution use dedicated
         * workflow endpoints.
         */

        await _ticketRepository
            .UpdateAsync(ticket);

        await _ticketRepository
            .SaveChangesAsync();

        await _activityLogRepository.AddAsync(
            new ActivityLog
            {
                TicketId = ticket.Id,
                UserId = userId,
                Action = "TicketDetailsUpdated",
                Description =
                    "Ticket title, description, category, or priority was updated.",
                PreviousValue =
                    $"Title: {oldTitle}; " +
                    $"CategoryId: {oldCategoryId}; " +
                    $"PriorityId: {oldPriorityId}",
                NewValue =
                    $"Title: {ticket.Title}; " +
                    $"CategoryId: {ticket.CategoryId}; " +
                    $"PriorityId: {ticket.PriorityId}",
                CreatedDate =
                    DateTime.UtcNow
            }
        );

        return (
            true,
            "Ticket updated successfully."
        );
    }
    public async Task<List<ActivityLogResponseDto>> GetTicketActivityAsync(int ticketId)
    {
        var logs = await _activityLogRepository.GetActivityLogsAsync(ticketId);

        return logs.Select(log => new ActivityLogResponseDto
        {
            User = $"{log.User.FirstName} {log.User.LastName}",
            Action = log.Action,
            CreatedDate = log.CreatedDate

        }).ToList();

    }

    public async Task<(bool Success, string Message)>
DeleteTicketAsync(
    int id,
    int userId,
    string role
)
    {
        if (role != "Admin")
        {
            return (
                false,
                "Only an Admin can permanently delete a ticket."
            );
        }

        var ticket =
            await _ticketRepository
                .GetByIdAsync(id);

        if (ticket == null)
        {
            return (
                false,
                "Ticket not found."
            );
        }

        if (
            ticket.StatusId ==
                TicketStatusIds.InProgress
        )
        {
            return (
                false,
                "An In Progress ticket cannot be deleted."
            );
        }

        string referenceNumber =
            ticket.ReferenceNumber;

        await _ticketRepository
            .DeleteAsync(ticket);

        await _ticketRepository
            .SaveChangesAsync();

        /*
         * This log is not connected to the deleted
         * ticket because the ticket no longer exists.
         */
        await _activityLogRepository.AddAsync(
            new ActivityLog
            {
                UserId = userId,
                TicketId = null,
                Action = "TicketDeleted",
                Description =
                    $"Ticket {referenceNumber} was permanently deleted by an Admin.",
                CreatedDate =
                    DateTime.UtcNow
            }
        );

        return (true,
            "Ticket deleted successfully."
        );

    }
    public async Task<List<TicketResponseDto>>
    FilterTicketsAsync(TicketFilterDto filter)
    {
        var tickets =
            await _ticketRepository.FilterTicketsAsync(filter);

        return tickets.Select(ticket =>
            new TicketResponseDto
            {
                Id = ticket.Id,

                Title = ticket.Title,
                Description = ticket.Description,

                CreatedByUserId =
                    ticket.CreatedByUserId,

                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                StatusId = ticket.StatusId,

                Category =
                    ticket.Category?.Name ?? "",

                Priority =
                    ticket.Priority?.Name ?? "",

                Status =
                    ticket.Status?.Name ?? "",

                ReferenceNumber =
                    ticket.ReferenceNumber,

                CreatedBy =
                    ticket.CreatedByUser == null
                        ? ""
                        : $"{ticket.CreatedByUser.FirstName} " +
                          $"{ticket.CreatedByUser.LastName}",

                CreatedDate = ticket.CreatedDate,
                DueDate = ticket.DueDate,
                ResolvedDate = ticket.ResolvedDate,
                ClosedDate = ticket.ClosedDate,
                AssignedToUserId =
    ticket.AssignedToUserId,

                AssignedTo =
    ticket.AssignedToUser == null
        ? null
        : $"{ticket.AssignedToUser.FirstName} " +
          $"{ticket.AssignedToUser.LastName}",
                Solution = ticket.Solution
            })
            .ToList();
    }
    public async Task<List<TicketResponseDto>>
    GetTicketsForUserAsync(
        int userId,
        string role)
    {
        var tickets =
            await _ticketRepository.GetTicketsForUserAsync(
                userId,
                role);

        return tickets.Select(ticket =>
            new TicketResponseDto
            {
                Id = ticket.Id,

                Title = ticket.Title,
                Description = ticket.Description,

                CreatedByUserId =
                    ticket.CreatedByUserId,

                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                StatusId = ticket.StatusId,

                Category =
                    ticket.Category?.Name ?? "",

                Priority =
                    ticket.Priority?.Name ?? "",

                Status =
                    ticket.Status?.Name ?? "",

                ReferenceNumber =
                    ticket.ReferenceNumber,

                CreatedBy =
                    ticket.CreatedByUser == null
                        ? ""
                        : $"{ticket.CreatedByUser.FirstName} " +
                          $"{ticket.CreatedByUser.LastName}",

                CreatedDate =
                    ticket.CreatedDate,

                DueDate =
                    ticket.DueDate,

                ResolvedDate =
                    ticket.ResolvedDate,

                ClosedDate =
                    ticket.ClosedDate,
                AssignedToUserId =
    ticket.AssignedToUserId,

                AssignedTo =
    ticket.AssignedToUser == null
        ? null
        : $"{ticket.AssignedToUser.FirstName} " +
          $"{ticket.AssignedToUser.LastName}",

                Solution =
                    ticket.Solution
            })
            .ToList();
    }
    public async Task<(bool Success, string Message)>
    StartWorkAsync(
        int ticketId,
        int agentId,
        StartTicketWorkRequestDto request
    )
    {
        var ticket =
            await _ticketRepository
                .GetByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket not found."
            );
        }

        if (
            ticket.AssignedToUserId !=
                agentId
        )
        {
            return (
                false,
                "You are not assigned to this ticket."
            );
        }

        bool canStart =
            ticket.StatusId ==
                TicketStatusIds.Assigned
            ||
            ticket.StatusId ==
                TicketStatusIds.Reopened;

        if (!canStart)
        {
            return (
                false,
                "Only Assigned or Reopened tickets can be started."
            );
        }

        var activeWorkLog =
            await _workLogRepository
                .GetActiveWorkLogAsync(
                    ticketId,
                    agentId
                );

        if (activeWorkLog != null)
        {
            return (
                false,
                "An active work session already exists for this ticket."
            );
        }

        string previousStatus =
            ticket.Status?.Name ??
            TicketStatusNames.Assigned;

        var workLog =
            new TicketWorkLog
            {
                TicketId = ticketId,
                AgentId = agentId,

                StartTime =
                    DateTime.UtcNow,

                EndTime = null,
                MinutesWorked = null,

                Description =
                    string.IsNullOrWhiteSpace(
                        request.Description
                    )
                        ? null
                        : request.Description
                            .Trim(),

                CreatedDate =
                    DateTime.UtcNow
            };

        await _workLogRepository
            .AddAsync(workLog);

        ticket.StatusId =
            TicketStatusIds.InProgress;

        await _ticketRepository
            .UpdateAsync(ticket);

        /*
         * TicketRepository and TicketWorkLogRepository
         * use the same scoped DbContext.
         *
         * This SaveChanges call saves both:
         * - the new work log
         * - the updated ticket
         */
        await _ticketRepository
            .SaveChangesAsync();
        await _ticketRepository
            .SaveChangesAsync();

        await _notificationService
            .NotifyTicketCreatorWorkStartedAsync(
                ticket.CreatedByUserId,
                ticket.Id,
                ticket.ReferenceNumber,
                ticket.Title
            );

        await _activityLogRepository.AddAsync(
            new ActivityLog
            {
                TicketId = ticketId,
                UserId = agentId,

                Action = "WorkStarted",

                PreviousValue =
                    previousStatus,

                NewValue =
                    TicketStatusNames.InProgress,

                Description =
                    string.IsNullOrWhiteSpace(
                        request.Description
                    )
                        ? "The assigned agent started working on the ticket."
                        : request.Description.Trim(),

                CreatedDate =
                    DateTime.UtcNow
            }
        );

        return (
            true,
            "Work started successfully."
        );
    }
    public async Task<bool> CanViewTicketAsync(
    int ticketId,
    int userId,
    string role
)
    {
        var ticket =
            await _ticketRepository
                .GetByIdAsync(ticketId);

        if (ticket == null)
        {
            return false;
        }

        // Admin and Manager can view every ticket.
        if (
            role == "Admin" ||
            role == "Manager"
        )
        {
            return true;
        }

        // Agents can view:
        // 1. Tickets assigned to them.
        // 2. Open and unassigned tickets.
        if (role == "IT Support Agent")
        {
            bool assignedToAgent =
                ticket.AssignedToUserId == userId;

            bool availableForRequest =
                ticket.AssignedToUserId == null &&
                ticket.StatusId ==
                    TicketStatusIds.Open;

            return
                assignedToAgent ||
                availableForRequest;
        }

        if (role == "Employee")
        {
            bool ownsTicket =
                ticket.CreatedByUserId == userId;

            bool completedTicket =
                ticket.StatusId ==
                    TicketStatusIds.Resolved
                ||
                ticket.StatusId ==
                    TicketStatusIds.Closed;

            bool hasPublishedSolution =
                !string.IsNullOrWhiteSpace(
                    ticket.Solution
                );

            return
                ownsTicket ||
                (
                    completedTicket &&
                    hasPublishedSolution
                );
        }

        return false;
    }
    public async Task<(bool Success, string Message)>
    ResolveTicketAsync(
        int ticketId,
        int agentId,
        ResolveTicketRequestDto request
    )
    {
        var ticket =
            await _ticketRepository
                .GetByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket not found."
            );
        }

        if (
            ticket.AssignedToUserId !=
            agentId
        )
        {
            return (
                false,
                "You are not assigned to this ticket."
            );
        }

        if (
            ticket.StatusId !=
            TicketStatusIds.InProgress
        )
        {
            return (
                false,
                "Only an In Progress ticket can be resolved."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                request.Solution
            )
        )
        {
            return (
                false,
                "A solution is required before resolving the ticket."
            );
        }

        if (
            request.Solution.Trim().Length < 10
        )
        {
            return (
                false,
                "The solution must contain at least 10 characters."
            );
        }

        var activeWorkLog =
            await _workLogRepository
                .GetActiveWorkLogAsync(
                    ticketId,
                    agentId
                );

        if (activeWorkLog == null)
        {
            return (
                false,
                "No active work session was found. Start work before resolving the ticket."
            );
        }

        DateTime resolvedTime =
            DateTime.UtcNow;

        activeWorkLog.EndTime =
            resolvedTime;

        double totalMinutes =
            (
                resolvedTime -
                activeWorkLog.StartTime
            ).TotalMinutes;

        /*
         * Math.Ceiling ensures that a partial minute
         * is recorded as one complete minute.
         */
        activeWorkLog.MinutesWorked =
            Math.Max(
                1,
                (int)Math.Ceiling(
                    totalMinutes
                )
            );

        if (
            !string.IsNullOrWhiteSpace(
                request.WorkDescription
            )
        )
        {
            activeWorkLog.Description =
                request.WorkDescription.Trim();
        }

        ticket.Solution =
            request.Solution.Trim();

        ticket.ResolvedDate =
            resolvedTime;

        ticket.ClosedDate = null;

        ticket.StatusId =
            TicketStatusIds.Resolved;

        await _ticketRepository
            .UpdateAsync(ticket);

        /*
         * The ticket and activeWorkLog are both tracked
         * by the same scoped ApplicationDbContext.
         *
         * Saving through the ticket repository persists:
         * - ticket changes
         * - work-log changes
         */
        await _ticketRepository
            .SaveChangesAsync();

        await _activityLogRepository.AddAsync(
            new ActivityLog
            {
                TicketId =
                    ticket.Id,

                UserId =
                    agentId,

                Action =
                    "TicketResolved",

                PreviousValue =
                    TicketStatusNames.InProgress,

                NewValue =
                    TicketStatusNames.Resolved,

                Description =
                    $"Ticket resolved. Actual work time: " +
                    $"{activeWorkLog.MinutesWorked} minute(s).",

                CreatedDate =
                    resolvedTime
            }
        );
        await _notificationService
            .NotifyTicketResolvedAsync(
                ticket.CreatedByUserId,
                ticket.Id,
                ticket.ReferenceNumber,
                ticket.Title
            );
        return (
            true,
            "Ticket resolved successfully."
        );
    }
    public async Task<(bool Success, string Message)>
    CloseTicketAsync(
        int ticketId,
        int userId,
        string role,
        CloseTicketRequestDto request
    )
    {
        var ticket =
            await _ticketRepository
                .GetByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket not found."
            );
        }

        if (
            ticket.StatusId !=
            TicketStatusIds.Resolved
        )
        {
            return (
                false,
                "Only a Resolved ticket can be closed."
            );
        }

        bool isAdmin =
            role == "Admin";

        bool isManager =
            role == "Manager";

        bool isEmployeeOwner =
            role == "Employee"
            &&
            ticket.CreatedByUserId ==
                userId;

        bool canClose =
            isAdmin
            ||
            isManager
            ||
            isEmployeeOwner;

        if (!canClose)
        {
            return (
                false,
                "You are not allowed to close this ticket."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                request.Reason
            )
        )
        {
            return (
                false,
                "A closing reason is required."
            );
        }

        if (
            request.Reason.Trim().Length <
            5
        )
        {
            return (
                false,
                "The closing reason must contain at least 5 characters."
            );
        }

        DateTime closedTime =
            DateTime.UtcNow;

        ticket.StatusId =
            TicketStatusIds.Closed;

        ticket.ClosedDate =
            closedTime;

        await _ticketRepository
            .UpdateAsync(ticket);

        await _ticketRepository
            .SaveChangesAsync();

        await _activityLogRepository.AddAsync(
            new ActivityLog
            {
                TicketId =
                    ticket.Id,

                UserId =
                    userId,

                Action =
                    "TicketClosed",

                Description =
                    request.Reason.Trim(),

                PreviousValue =
                    TicketStatusNames.Resolved,

                NewValue =
                    TicketStatusNames.Closed,

                CreatedDate =
                    closedTime
            }
        );
        await _notificationService
            .NotifyTicketClosedAsync(
                ticket.CreatedByUserId,
                ticket.AssignedToUserId,
                userId,
                ticket.Id,
                ticket.ReferenceNumber,
                ticket.Title
            );
        return (
            true,
            "Ticket closed successfully."
        );
    }
    public async Task<(bool Success, string Message)>
    PublishTicketAsync(
        int ticketId,
        int publishedByUserId,
        PublishTicketRequestDto request
    )
    {
        var ticket =
            await _ticketRepository
                .GetByIdAsync(ticketId);

        if (ticket == null)
        {
            return (
                false,
                "Ticket not found."
            );
        }

        if (ticket.AssignedToUserId.HasValue)
        {
            return (
                false,
                "An assigned ticket cannot be published."
            );
        }

        if (
            ticket.StatusId !=
            TicketStatusIds.PendingReview
        )
        {
            return (
                false,
                "Only Pending Review tickets can be published."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                request.Notes
            )
        )
        {
            return (
                false,
                "Publishing notes are required."
            );
        }

        string notes =
            request.Notes.Trim();

        if (notes.Length < 5)
        {
            return (
                false,
                "Publishing notes must contain at least 5 characters."
            );
        }

        DateTime publishedDate =
            DateTime.UtcNow;

        ticket.StatusId =
            TicketStatusIds.Open;

        await _ticketRepository
            .UpdateAsync(ticket);

        await _ticketRepository
            .SaveChangesAsync();

        await _activityLogRepository.AddAsync(
            new ActivityLog
            {
                TicketId =
                    ticket.Id,

                UserId =
                    publishedByUserId,

                Action =
                    "TicketPublished",

                Description =
                    notes,

                PreviousValue =
                    TicketStatusNames.PendingReview,

                NewValue =
                    TicketStatusNames.Open,

                CreatedDate =
                    DateTime.UtcNow
            }
        );

        await _notificationService
            .NotifyAgentsTicketPublishedAsync(
                ticket.Id,
                ticket.ReferenceNumber,
                ticket.Title
            );

        return (
            true,
            "Ticket published successfully. IT Support Agents can now request it."
        );
    }
}