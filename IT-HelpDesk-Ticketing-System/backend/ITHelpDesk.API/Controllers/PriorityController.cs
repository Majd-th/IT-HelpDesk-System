using Microsoft.AspNetCore.Mvc;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Models;

namespace ITHelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriorityController : ControllerBase
{
    private readonly IPriorityService _service;

    public PriorityController(IPriorityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var priority = await _service.GetByIdAsync(id);

        if (priority == null)
            return NotFound();

        return Ok(priority);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Priority priority)
    {
        var created = await _service.CreateAsync(priority);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Priority priority)
    {
        var success = await _service.UpdateAsync(id, priority);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}