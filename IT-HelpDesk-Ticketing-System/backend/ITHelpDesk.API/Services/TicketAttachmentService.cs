using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.AspNetCore.Http;

namespace ITHelpDesk.API.Services;

public class TicketAttachmentService : ITicketAttachmentService
{
    private readonly ITicketAttachmentRepository _repository;
    private readonly IWebHostEnvironment _environment;

    public TicketAttachmentService(
        ITicketAttachmentRepository repository,
        IWebHostEnvironment environment)
    {
        _repository = repository;
        _environment = environment;
    }

    public async Task UploadAsync(
        int ticketId,
        IFormFile file,
        int userId)
    {
        if (file == null || file.Length == 0)
            throw new Exception("No file uploaded.");

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "attachments");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName =
            Guid.NewGuid() +
            Path.GetExtension(file.FileName);

        var filePath =
            Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new TicketAttachment
        {
            TicketId = ticketId,
            UserId = userId,
            FileName = file.FileName,
            FilePath = uniqueFileName,
            FileSize = file.Length,
            ContentType = file.ContentType
        };

        await _repository.AddAsync(attachment);
    }

    public async Task<List<AttachmentResponseDto>> GetAllAsync(int ticketId)
    {
        var attachments =
            await _repository.GetByTicketIdAsync(ticketId);

        return attachments.Select(a => new AttachmentResponseDto
        {
            Id = a.Id,
            FileName = a.FileName,
            FileSize = a.FileSize,
            UploadedDate = a.UploadedDate
        }).ToList();
    }

    public async Task<List<AttachmentResponseDto>> GetAttachmentsAsync(int ticketId)
    {
        return await GetAllAsync(ticketId);
    }

    public async Task<(byte[] File, string ContentType, string FileName)>
        DownloadAsync(int attachmentId)
    {
        var attachment =
            await _repository.GetByIdAsync(attachmentId);

        if (attachment == null)
            throw new Exception("Attachment not found.");

        var fullPath = Path.Combine(
            _environment.WebRootPath,
            "attachments",
            attachment.FilePath);

        var bytes =
            await File.ReadAllBytesAsync(fullPath);

        return (
            bytes,
            attachment.ContentType,
            attachment.FileName
        );
    }

    public async Task<bool> DeleteAsync(
        int attachmentId,
        int userId)
    {
        var attachment =
            await _repository.GetByIdAsync(attachmentId);

        if (attachment == null)
            return false;

        if (attachment.UserId != userId)
            return false;

        var fullPath = Path.Combine(
            _environment.WebRootPath,
            "attachments",
            attachment.FilePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        await _repository.DeleteAsync(attachment);

        return true;
    }
}