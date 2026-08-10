using System.Security.Claims;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController
    : ControllerBase
{
    private readonly INotificationService
        _notificationService;

    public NotificationController(
        INotificationService
            notificationService)
    {
        _notificationService =
            notificationService;
    }

    // =====================================================
    // GET MY NOTIFICATIONS
    // =====================================================

    [HttpGet]
    public async Task<IActionResult>
        GetMyNotifications()
    {
        if (
            !TryGetCurrentUserId(
                out int userId
            )
        )
        {
            return Unauthorized(new
            {
                message =
                    "The authenticated user could not be identified."
            });
        }

        var notifications =
            await _notificationService
                .GetUserNotificationsAsync(
                    userId
                );

        return Ok(notifications);
    }

    // =====================================================
    // GET UNREAD COUNT
    // =====================================================

    [HttpGet("unread-count")]
    public async Task<IActionResult>
        GetUnreadCount()
    {
        if (
            !TryGetCurrentUserId(
                out int userId
            )
        )
        {
            return Unauthorized();
        }

        int count =
            await _notificationService
                .GetUnreadCountAsync(
                    userId
                );

        return Ok(new
        {
            unreadCount =
                count
        });
    }

    // =====================================================
    // MARK ONE AS READ
    // =====================================================

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult>
        MarkAsRead(
            int id)
    {
        if (
            !TryGetCurrentUserId(
                out int userId
            )
        )
        {
            return Unauthorized();
        }

        var result =
            await _notificationService
                .MarkAsReadAsync(
                    id,
                    userId
                );

        if (!result.Success)
        {
            return BadRequest(new
            {
                message =
                    result.Message
            });
        }

        return Ok(new
        {
            message =
                result.Message
        });
    }

    // =====================================================
    // MARK ALL AS READ
    // =====================================================

    [HttpPut("read-all")]
    public async Task<IActionResult>
        MarkAllAsRead()
    {
        if (
            !TryGetCurrentUserId(
                out int userId
            )
        )
        {
            return Unauthorized();
        }

        var result =
            await _notificationService
                .MarkAllAsReadAsync(
                    userId
                );

        return Ok(new
        {
            message =
                result.Message
        });
    }

    // =====================================================
    // CURRENT USER
    // =====================================================

    private bool TryGetCurrentUserId(
        out int userId)
    {
        userId = 0;

        string? userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            )
            ??
            User.FindFirstValue(
                "userId"
            )
            ??
            User.FindFirstValue(
                "id"
            )
            ??
            User.FindFirstValue(
                "sub"
            );

        return int.TryParse(
            userIdValue,
            out userId
        );
    }
}