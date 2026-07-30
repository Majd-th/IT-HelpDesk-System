using ITHelpDesk.API.DTOs;
using Microsoft.AspNetCore.Http;

namespace ITHelpDesk.API.Interfaces;

public interface ITicketAttachmentService
{
    Task UploadAsync(
        int ticketId,
        IFormFile file,
        int userId
    );

    Task<List<AttachmentResponseDto>>
        GetAttachmentsAsync(int ticketId);

    Task<(
        byte[] File,
        string ContentType,
        string FileName
    )> DownloadAsync(int attachmentId);

    Task<bool> DeleteAsync(
        int attachmentId,
        int userId,
        string role
    );
}