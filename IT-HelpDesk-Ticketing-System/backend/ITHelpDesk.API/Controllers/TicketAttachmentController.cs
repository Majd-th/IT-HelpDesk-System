using System.Security.Claims;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketAttachmentController : ControllerBase
{
    private readonly ITicketAttachmentService _attachmentService;

    public TicketAttachmentController(
        ITicketAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    [HttpPost("{ticketId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
     int ticketId,
     IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Please select a file."
            });
        }

        int userId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!
        );

        try
        {
            await _attachmentService.UploadAsync(
                ticketId,
                file,
                userId
            );

            return Ok(new
            {
                message = "Attachment uploaded successfully."
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet("ticket/{ticketId}")]
    public async Task<IActionResult> GetByTicket(
        int ticketId)
    {
        var attachments =
            await _attachmentService
                .GetAttachmentsAsync(ticketId);

        return Ok(attachments);
    }

    [HttpGet("download/{attachmentId}")]
    public async Task<IActionResult> Download(
        int attachmentId)
    {
        try
        {
            var file =
                await _attachmentService
                    .DownloadAsync(attachmentId);

            return File(
                file.File,
                file.ContentType,
                file.FileName
            );
        }
        catch (FileNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }

    [HttpDelete("{attachmentId}")]
    public async Task<IActionResult> Delete(
        int attachmentId)
    {
        int userId = int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!
        );

        string role =
            User.FindFirstValue(
                ClaimTypes.Role) ?? "";

        bool deleted =
            await _attachmentService.DeleteAsync(
                attachmentId,
                userId,
                role
            );

        if (!deleted)
        {
            return BadRequest(new
            {
                message =
                    "You cannot delete this attachment."
            });
        }

        return NoContent();
    }
}