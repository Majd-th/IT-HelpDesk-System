using System.Security.Claims;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController
    : ControllerBase
{
    private readonly IDashboardService
        _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService =
            dashboardService;
    }

    // =====================================================
    // GET ROLE-BASED DASHBOARD ANALYTICS
    // =====================================================

    [HttpGet("analytics")]
    public async Task<IActionResult>
        GetAnalytics(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
    {
        if (
            !TryGetCurrentUser(
                out int userId,
                out string role
            )
        )
        {
            return Unauthorized(new
            {
                message =
                    "The authenticated user could not be identified."
            });
        }

        DateTime today =
     DateTime.UtcNow.Date;

        DateTime requestedToDate =
            to?.Date ??
            today;

        /*
         * Dashboard analytics should not generate
         * future trend points.
         */
        DateTime toDate =
            requestedToDate > today
                ? today
                : requestedToDate;

        DateTime fromDate =
            from?.Date ??
            toDate.AddDays(-29);
        if (fromDate > toDate)
        {
            return BadRequest(new
            {
                message =
                    "The from date cannot be after the to date."
            });
        }

        if (
            (
                toDate -
                fromDate
            ).TotalDays > 730
        )
        {
            return BadRequest(new
            {
                message =
                    "The dashboard date range cannot exceed two years."
            });
        }

        var analytics =
            await _dashboardService
                .GetAnalyticsAsync(
                    userId,
                    role,
                    fromDate,
                    toDate
                );

        return Ok(analytics);
    }

    private bool TryGetCurrentUser(
        out int userId,
        out string role)
    {
        userId = 0;
        role = string.Empty;

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

        string? roleValue =
            User.FindFirstValue(
                ClaimTypes.Role
            )
            ??
            User.FindFirstValue(
                "role"
            );

        if (
            !int.TryParse(
                userIdValue,
                out userId
            )
            ||
            string.IsNullOrWhiteSpace(
                roleValue
            )
        )
        {
            return false;
        }

        role =
            roleValue;

        return true;
    }
}