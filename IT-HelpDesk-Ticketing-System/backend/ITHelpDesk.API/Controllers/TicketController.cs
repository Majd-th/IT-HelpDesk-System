using System.Security.Claims;
using ITHelpDesk.API.DTOs;
using ITHelpDesk.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketRequestDto request)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var ticket = await _ticketService.CreateTicketAsync(request, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = ticket.Id },
            ticket);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();

        return Ok(tickets);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);

        if (ticket == null)
            return NotFound();

        return Ok(ticket);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    int id,
    UpdateTicketRequestDto request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;

        var updated = await _ticketService.UpdateTicketAsync(
    id,
    request,
    userId,
    role);

        if (!updated)
            return NotFound();

        return NoContent();
    }/*
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;


        var deleted = await _ticketService.DeleteTicketAsync(id, userId, role);

        if (!deleted)
            return NotFound();

        return NoContent();
    }*/
    [HttpGet("{id}/activity")]
    public async Task<IActionResult> GetActivity(int id)
    {
        var activity = await _ticketService.GetTicketActivityAsync(id);

        return Ok(activity);
    }
    [HttpGet("filter")]
    public async Task<IActionResult> Filter([FromQuery] TicketFilterDto filter)
    {
        var tickets = await _ticketService.FilterTicketsAsync(
            filter.CategoryId,
            filter.PriorityId,
            filter.StatusId,
            filter.CreatedAfter,
            filter.CreatedBefore);

        return Ok(tickets);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        Console.WriteLine($"DELETE endpoint hit. Ticket Id = {id}");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        Console.WriteLine($"UserId = {userId}");
        Console.WriteLine($"Role = {role}");

        var deleted = await _ticketService.DeleteTicketAsync(id, userId, role);

        Console.WriteLine($"Deleted = {deleted}");

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}