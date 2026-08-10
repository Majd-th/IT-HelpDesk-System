using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;
using Microsoft.AspNetCore.Http;

namespace ITHelpDesk.API.Services;

public class TicketAttachmentService
    : ITicketAttachmentService
{
    private readonly
        ITicketAttachmentRepository
        _repository;

    private readonly
        IWebHostEnvironment
        _environment;


    public TicketAttachmentService(
        ITicketAttachmentRepository repository,
        IWebHostEnvironment environment)
    {
        _repository =
            repository;

        _environment =
            environment;
    }


    public async Task UploadAsync(
        int ticketId,
        IFormFile file,
        int userId)
    {
        if (
            file == null ||
            file.Length == 0
        )
        {
            throw new ArgumentException(
                "No file was uploaded."
            );
        }


        const long maximumFileSize =
            10 * 1024 * 1024;


        if (
            file.Length >
            maximumFileSize
        )
        {
            throw new ArgumentException(
                "The maximum attachment size is 10 MB."
            );
        }


        string[] allowedExtensions =
        {
            ".txt",
            ".docx",
            ".xlsx",
            ".pdf",
            ".png",
            ".jpg",
            ".jpeg"
        };


        string extension =
            Path.GetExtension(
                file.FileName
            )
            .ToLowerInvariant();


        if (
            !allowedExtensions.Contains(
                extension
            )
        )
        {
            throw new ArgumentException(
                "This file type is not allowed."
            );
        }


        string uploadsFolder =
            GetUploadsFolder();


        Directory.CreateDirectory(
            uploadsFolder
        );


        string uniqueFileName =
            $"{Guid.NewGuid()}{extension}";


        string filePath =
            Path.Combine(
                uploadsFolder,
                uniqueFileName
            );


        await using (
            var stream =
                new FileStream(
                    filePath,
                    FileMode.CreateNew
                )
        )
        {
            await file.CopyToAsync(
                stream
            );
        }


        var attachment =
            new TicketAttachment
            {
                TicketId =
                    ticketId,

                UserId =
                    userId,

                FileName =
                    Path.GetFileName(
                        file.FileName
                    ),

                FilePath =
                    uniqueFileName,

                FileSize =
                    file.Length,

                ContentType =
                    string.IsNullOrWhiteSpace(
                        file.ContentType
                    )
                    ? "application/octet-stream"
                    : file.ContentType,

                UploadedDate =
                    DateTime.UtcNow
            };


        await _repository
            .AddAsync(
                attachment
            );
    }


    public async Task<
        List<AttachmentResponseDto>>
        GetAllAsync(
            int ticketId)
    {
        var attachments =
            await _repository
                .GetByTicketIdAsync(
                    ticketId
                );


        return attachments
            .Select(
                attachment =>
                    new AttachmentResponseDto
                    {
                        Id =
                            attachment.Id,

                        FileName =
                            attachment.FileName,

                        FileSize =
                            attachment.FileSize,

                        UploadedDate =
                            attachment.UploadedDate,

                        UserId =
                            attachment.UserId
                    }
            )
            .ToList();
    }


    public async Task<
        List<AttachmentResponseDto>>
        GetAttachmentsAsync(
            int ticketId)
    {
        return await GetAllAsync(
            ticketId
        );
    }


    public async Task<(
        byte[] File,
        string ContentType,
        string FileName
    )>
    DownloadAsync(
        int attachmentId)
    {
        var attachment =
            await _repository
                .GetByIdAsync(
                    attachmentId
                );


        if (attachment == null)
        {
            throw new FileNotFoundException(
                "Attachment record was not found."
            );
        }


        string fullPath =
            Path.Combine(
                GetUploadsFolder(),
                attachment.FilePath
            );


        if (
            !File.Exists(
                fullPath
            )
        )
        {
            throw new FileNotFoundException(
                "The attachment file does not exist on the server."
            );
        }


        byte[] bytes =
            await File.ReadAllBytesAsync(
                fullPath
            );


        string contentType =
            string.IsNullOrWhiteSpace(
                attachment.ContentType
            )
            ? "application/octet-stream"
            : attachment.ContentType;


        return (
            bytes,
            contentType,
            attachment.FileName
        );
    }


    public async Task<bool>
        DeleteAsync(
            int attachmentId,
            int userId,
            string role)
    {
        var attachment =
            await _repository
                .GetByIdAsync(
                    attachmentId
                );


        if (attachment == null)
        {
            return false;
        }


        bool isUploader =
            attachment.UserId ==
            userId;


        bool isAdmin =
            string.Equals(
                role,
                "Admin",
                StringComparison
                    .OrdinalIgnoreCase
            );


        if (
            !isUploader &&
            !isAdmin
        )
        {
            return false;
        }


        string fullPath =
            Path.Combine(
                GetUploadsFolder(),
                attachment.FilePath
            );


        if (
            File.Exists(
                fullPath
            )
        )
        {
            File.Delete(
                fullPath
            );
        }


        await _repository
            .DeleteAsync(
                attachment
            );


        return true;
    }


    private string GetUploadsFolder()
    {
        return Path.Combine(
            _environment.ContentRootPath,
            "Storage",
            "attachments"
        );
    }
}