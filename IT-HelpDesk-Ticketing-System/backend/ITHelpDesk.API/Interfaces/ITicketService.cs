using ITHelpDesk.API.DTOs;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketService
{
    Task<TicketResponseDto>
        CreateTicketAsync(
            CreateTicketRequestDto request,
            int userId
        );

    Task<List<TicketResponseDto>>
        GetAllTicketsAsync();

    Task<TicketResponseDto?>
        GetTicketByIdAsync(int id);

    Task<bool> CanViewTicketAsync(
        int ticketId,
        int userId,
        string role
    );

    Task<(bool Success, string Message)>
        UpdateTicketAsync(
            int id,
            UpdateTicketRequestDto request,
            int userId,
            string role
        );

    Task<(bool Success, string Message)>
        DeleteTicketAsync(
            int id,
            int userId,
            string role
        );

    Task<List<ActivityLogResponseDto>>
        GetTicketActivityAsync(
            int ticketId
        );

    Task<List<TicketResponseDto>>
        FilterTicketsAsync(
            TicketFilterDto filter
        );

    Task<List<TicketResponseDto>>
        GetTicketsForUserAsync(
            int userId,
            string role
        );

    Task<(bool Success, string Message)>
        StartWorkAsync(
            int ticketId,
            int agentId,
            StartTicketWorkRequestDto request
        );
    Task<(bool Success, string Message)>
ResolveTicketAsync(
    int ticketId,
    int agentId,
    ResolveTicketRequestDto request
);
    Task<(bool Success, string Message)>
        CloseTicketAsync(
            int ticketId,
            int userId,
            string role,
            CloseTicketRequestDto request
        );
    Task<(bool Success, string Message)>
PublishTicketAsync(
    int ticketId,
    int publishedByUserId,
    PublishTicketRequestDto request
);
}