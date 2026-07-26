using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

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
    public async Task<IActionResult> Upload(
     int ticketId,
     IFormFile file)
    {
        int userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _attachmentService.UploadAsync(
            ticketId,
            file,
            userId);

        return Ok("File uploaded successfully.");
    }

    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetAll(int ticketId)
    {
        return Ok(await _attachmentService.GetAllAsync(ticketId));
    }
    [HttpDelete("{attachmentId}")]
    public async Task<IActionResult> Delete(int attachmentId)
    {
        int userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        bool success =
            await _attachmentService.DeleteAsync(
                attachmentId,
                userId);

        if (!success)
            return NotFound();

        return NoContent();
    }
    [HttpGet("download/{attachmentId}")]
    public async Task<IActionResult> Download(int attachmentId)
    {
        var file = await _attachmentService.DownloadAsync(
            attachmentId);

        return File(
            file.File,
            file.ContentType,
            file.FileName);
    }
}