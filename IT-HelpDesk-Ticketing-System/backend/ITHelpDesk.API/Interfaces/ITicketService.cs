using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Models;
namespace ITHelpDesk.API.Interfaces;

public interface ITicketService
{
    Task<TicketResponseDto> CreateTicketAsync(
        CreateTicketRequestDto request,
        int userId);

    Task<List<TicketResponseDto>> GetAllTicketsAsync();

    Task<TicketResponseDto?> GetTicketByIdAsync(int id);
    Task<bool> UpdateTicketAsync(
        int id,
        UpdateTicketRequestDto request,
        int userId,
        string role);

    Task<bool> DeleteTicketAsync(
        int id,
        int userId,
        string role);
    Task<List<ActivityLogResponseDto>> GetTicketActivityAsync(int ticketId);
    Task<List<TicketResponseDto>> FilterTicketsAsync(
    int? categoryId,
    int? priorityId,
    int? statusId,
    DateTime? createdAfter,
    DateTime? createdBefore);
}