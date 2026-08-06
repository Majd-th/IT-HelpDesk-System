using System.Security.Claims;
using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController
    : ControllerBase
{
    private readonly ITicketService
        _ticketService;

    public TicketController(
        ITicketService ticketService
    )
    {
        _ticketService =
            ticketService;
    }

    // =====================================================
    // CREATE TICKET
    // Employee and Admin only
    // =====================================================

    [HttpPost]
    [Authorize(Roles = "Employee,Admin")]
    public async Task<IActionResult> Create(
        CreateTicketRequestDto request
    )
    {
        if (!TryGetCurrentUser(
            out int userId,
            out _))
        {
            return Unauthorized();
        }

        var ticket =
            await _ticketService
                .CreateTicketAsync(
                    request,
                    userId
                );

        return CreatedAtAction(
            nameof(GetById),
            new { id = ticket.Id },
            ticket
        );
    }

    // =====================================================
    // ROLE-SPECIFIC TICKET LIST
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetCurrentUser(
            out int userId,
            out string role))
        {
            return Unauthorized();
        }

        var tickets =
            await _ticketService
                .GetTicketsForUserAsync(
                    userId,
                    role
                );

        return Ok(tickets);
    }

    // =====================================================
    // VIEW ONE TICKET
    // Includes object-level permission check
    // =====================================================

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        int id
    )
    {
        if (!TryGetCurrentUser(
            out int userId,
            out string role))
        {
            return Unauthorized();
        }

        bool canView =
            await _ticketService
                .CanViewTicketAsync(
                    id,
                    userId,
                    role
                );

        if (!canView)
        {
            return StatusCode(
                StatusCodes
                    .Status403Forbidden,
                new
                {
                    message =
                        "You are not allowed to view this ticket."
                }
            );
        }

        var ticket =
            await _ticketService
                .GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return NotFound(new
            {
                message =
                    "Ticket not found."
            });
        }

        return Ok(ticket);
    }

    // =====================================================
    // EDIT TICKET DETAILS
    // Agent cannot use generic editing
    // =====================================================

    [HttpPut("{id}")]
    [Authorize(
        Roles =
            "Employee,Manager,Admin"
    )]
    public async Task<IActionResult> Update(
        int id,
        UpdateTicketRequestDto request
    )
    {
        if (!TryGetCurrentUser(
            out int userId,
            out string role))
        {
            return Unauthorized();
        }

        var result =
            await _ticketService
                .UpdateTicketAsync(
                    id,
                    request,
                    userId,
                    role
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
    // START WORK
    // Assigned IT Support Agent only
    // =====================================================

    [HttpPut("{id}/start-work")]
    [Authorize(
        Roles =
            "IT Support Agent"
    )]
    public async Task<IActionResult> StartWork(
        int id,
        StartTicketWorkRequestDto request
    )
    {
        if (!TryGetCurrentUser(
            out int agentId,
            out _))
        {
            return Unauthorized();
        }

        var result =
            await _ticketService
                .StartWorkAsync(
                    id,
                    agentId,
                    request
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
    // ACTIVITY HISTORY
    // User must be allowed to view the ticket
    // =====================================================

    [HttpGet("{id}/activity")]
    public async Task<IActionResult> GetActivity(
        int id
    )
    {
        if (!TryGetCurrentUser(
            out int userId,
            out string role))
        {
            return Unauthorized();
        }

        bool canView =
            await _ticketService
                .CanViewTicketAsync(
                    id,
                    userId,
                    role
                );

        if (!canView)
        {
            return StatusCode(
                StatusCodes
                    .Status403Forbidden,
                new
                {
                    message =
                        "You are not allowed to view this ticket's activity."
                }
            );
        }

        var activity =
            await _ticketService
                .GetTicketActivityAsync(id);

        return Ok(activity);
    }

    // =====================================================
    // FILTER
    // Temporarily limited to Manager/Admin
    // Other roles can filter their role-specific list
    // in React.
    // =====================================================

    [HttpGet("filter")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Filter(
        [FromQuery]
        TicketFilterDto filter
    )
    {
        var tickets =
            await _ticketService
                .FilterTicketsAsync(filter);

        return Ok(tickets);
    }

    // =====================================================
    // PERMANENT DELETE
    // Admin only
    // =====================================================

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id
    )
    {
        if (!TryGetCurrentUser(
            out int userId,
            out string role))
        {
            return Unauthorized();
        }

        var result =
            await _ticketService
                .DeleteTicketAsync(
                    id,
                    userId,
                    role
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
    // JWT HELPER
    // =====================================================

    private bool TryGetCurrentUser(
        out int userId,
        out string role
    )
    {
        string? userIdText =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        role =
            User.FindFirstValue(
                ClaimTypes.Role
            ) ?? "";

        return int.TryParse(
            userIdText,
            out userId
        );
    }
    // =====================================================
    // RESOLVE TICKET
    // Assigned IT Support Agent only
    // =====================================================

    [HttpPut("{id}/resolve")]
    [Authorize(Roles = "IT Support Agent")]
    public async Task<IActionResult> Resolve(
        int id,
        ResolveTicketRequestDto request
    )
    {
        if (!TryGetCurrentUser(
            out int agentId,
            out _))
        {
            return Unauthorized(new
            {
                message =
                    "The authenticated user could not be identified."
            });
        }

        var result =
            await _ticketService
                .ResolveTicketAsync(
                    id,
                    agentId,
                    request
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
    // CLOSE TICKET
    // Employee owner, Manager, or Admin
    // =====================================================

    [HttpPut("{id}/close")]
    [Authorize(
        Roles = "Employee,Manager,Admin"
    )]
    public async Task<IActionResult> CloseTicket(
        int id,
        CloseTicketRequestDto request
    )
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

        var result =
            await _ticketService
                .CloseTicketAsync(
                    id,
                    userId,
                    role,
                    request
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
    // PUBLISH TICKET
    // Pending Review -> Open
    // Manager and Admin only
    // =====================================================

    [HttpPut("{id:int}/publish")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> PublishTicket(
        int id,
        [FromBody] PublishTicketRequestDto request
    )
    {
        if (
            !TryGetCurrentUser(
                out int userId,
                out _
            )
        )
        {
            return Unauthorized(new
            {
                message =
                    "The authenticated user could not be identified."
            });
        }

        var result =
            await _ticketService
                .PublishTicketAsync(
                    id,
                    userId,
                    request
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
}