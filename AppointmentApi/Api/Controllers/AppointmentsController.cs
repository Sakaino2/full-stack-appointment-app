namespace Api.Controllers;

using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires Admin JWT authentication
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;

    /// <summary>
    /// Gets upcoming appointments for the dashboard.
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetUpcoming([FromQuery] int count = 10, CancellationToken ct = default)
    {
        var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(count, ct);
        return Ok(appointments);
    }

    /// <summary>
    /// Gets a single appointment by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var appointment = await _appointmentService.GetByIdAsync(id, ct);
        if (appointment is null) return NotFound();

        return Ok(appointment);
    }

    /// <summary>
    /// Creates a new appointment and syncs to Google Calendar.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> Create([FromBody] CreateAppointmentDto dto, CancellationToken ct)
    {
        if (dto.EndTime <= dto.StartTime)
        {
            return BadRequest("EndTime must be later than StartTime.");
        }

        try
        {
            var result = await _appointmentService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Reschedules an existing appointment.
    /// </summary>
    [HttpPatch("{id:guid}/reschedule")]
    public async Task<ActionResult<AppointmentResponseDto>> Reschedule(Guid id, [FromBody] RescheduleAppointmentDto dto, CancellationToken ct)
    {
        if (dto.NewEndTime <= dto.NewStartTime)
        {
            return BadRequest("NewEndTime must be later than NewStartTime.");
        }

        var result = await _appointmentService.RescheduleAsync(id, dto, ct);
        if (result is null) return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Full update of an appointment.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AppointmentResponseDto>> Update(Guid id, [FromBody] UpdateAppointmentDto dto, CancellationToken ct)
    {
        if (dto.EndTime <= dto.StartTime)
        {
            return BadRequest("EndTime must be later than StartTime.");
        }

        var result = await _appointmentService.UpdateAsync(id, dto, ct);
        if (result is null) return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Deletes an appointment and removes it from Google Calendar.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _appointmentService.DeleteAsync(id, ct);
        if (!success) return NotFound();

        return NoContent();
    }
}