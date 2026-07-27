using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;

    private readonly IActivityLogRepository _activityLogRepository;

    public TicketService(
        ITicketRepository ticketRepository,
        IActivityLogRepository activityLogRepository)
    {
        _ticketRepository = ticketRepository;
        _activityLogRepository = activityLogRepository;
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

            StatusId = 1,

            CreatedDate = DateTime.UtcNow
        };

        await _ticketRepository.AddAsync(ticket);
        await _ticketRepository.SaveChangesAsync();
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



            CreatedBy = $"{ticket.CreatedByUser.FirstName} {ticket.CreatedByUser.LastName}",

            CreatedDate = ticket.CreatedDate,
            DueDate = ticket.DueDate,
            ResolvedDate = ticket.ResolvedDate,
            ClosedDate = ticket.ClosedDate,

            Solution = ticket.Solution
        };
    }
    public async Task<bool> UpdateTicketAsync(
        int id,
        UpdateTicketRequestDto request,
        int userId,
        string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
            return false;

        if (ticket.Status?.Name == "Closed")
            return false;

        if (role == "Employee")
        {
            if (ticket.CreatedByUserId != userId)
                return false;

            if (ticket.StatusId != 1)
                return false;

            ticket.Title = request.Title;
            ticket.Description = request.Description;
        }
        else if (role == "IT Support Agent")
        {
            ticket.StatusId = request.StatusId;
            ticket.Solution = request.Solution;

            if (request.StatusId == 3)
                ticket.ResolvedDate = DateTime.UtcNow;

            if (request.StatusId == 4)
                ticket.ClosedDate = DateTime.UtcNow;
        }
        else if (role == "Manager" || role == "Admin")
        {
            ticket.Title = request.Title;
            ticket.Description = request.Description;

            ticket.CategoryId = request.CategoryId;
            ticket.PriorityId = request.PriorityId;
            ticket.StatusId = request.StatusId;

            ticket.Solution = request.Solution;

            if (request.StatusId == 3)
                ticket.ResolvedDate = DateTime.UtcNow;

            if (request.StatusId == 4)
                ticket.ClosedDate = DateTime.UtcNow;
        }
        else
        {
            return false;
        }
        await _ticketRepository.UpdateAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        await _activityLogRepository.AddAsync(new ActivityLog
        {
            TicketId = ticket.Id,
            UserId = userId,
            Action = "Updated ticket"
        });

        return true;
    }
    public async Task<bool> DeleteTicketAsync(int id, int userId, string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        Console.WriteLine($"Ticket: {ticket?.Id}");
        Console.WriteLine($"Role: {role}");
        Console.WriteLine($"User: {userId}");
        Console.WriteLine($"Status: {ticket?.StatusId}");
        Console.WriteLine($"Owner: {ticket?.CreatedByUserId}");
        if (ticket == null)
            return false;
        if (role == "Employee")
        {
            if (ticket.CreatedByUserId != userId)
                return false;

            if (ticket.StatusId != 1)
                return false;
        }
        else if (role == "IT Support Agent")
        {
            return false;
        }
        else if (role == "Manager" || role == "Admin")
        {
            // Managers and Admins can delete any ticket
        }
        else
        {
            return false;
        }
        // Only Open tickets can be deleted
        if (ticket.StatusId != 1)
            return false;
        await _activityLogRepository.DeleteByTicketIdAsync(ticket.Id);

        await _ticketRepository.DeleteAsync(ticket);
        await _ticketRepository.SaveChangesAsync();

        return true;


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
    public async Task<List<TicketResponseDto>> FilterTicketsAsync(
        int? categoryId,
        int? priorityId,
        int? statusId,
        DateTime? createdAfter,
        DateTime? createdBefore)
    {
        var filter = new TicketFilterDto
        {
            CategoryId = categoryId,
            PriorityId = priorityId,
            StatusId = statusId,
            CreatedAfter = createdAfter,
            CreatedBefore = createdBefore
        };

        var tickets = await _ticketRepository.FilterTicketsAsync(filter);

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


            CreatedBy = $"{ticket.CreatedByUser.FirstName} {ticket.CreatedByUser.LastName}",

            CreatedDate = ticket.CreatedDate,
            DueDate = ticket.DueDate,
            ResolvedDate = ticket.ResolvedDate,
            ClosedDate = ticket.ClosedDate,

            Solution = ticket.Solution

        }).ToList();
    }
}