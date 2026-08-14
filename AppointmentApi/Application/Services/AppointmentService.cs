namespace Application.Services;

using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AppointmentService(
    IApplicationDbContext context,
    IGoogleCalendarService googleCalendarService,
    ILogger<AppointmentService> logger) : IAppointmentService
{
    private readonly IApplicationDbContext _context = context;
    private readonly IGoogleCalendarService _googleCalendarService = googleCalendarService;
    private readonly ILogger<AppointmentService> _logger = logger;

    public async Task<IEnumerable<AppointmentResponseDto>> GetUpcomingAppointmentsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;

        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .Where(a => a.StartTime >= nowUtc && a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.StartTime)
            .Take(count)
            .Select(a => MapToResponseDto(a))
            .ToListAsync(cancellationToken);
    }

    public async Task<AppointmentResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return appointment is null ? null : MapToResponseDto(appointment);
    }

    public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _context.Clients.FindAsync(new object[] { dto.ClientId }, cancellationToken)
            ?? throw new KeyNotFoundException($"Client with ID '{dto.ClientId}' was not found.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ClientId = client.Id,
            Client = client,
            Title = dto.Title,
            Notes = dto.Notes,
            StartTime = dto.StartTime.ToUniversalTime(),
            EndTime = dto.EndTime.ToUniversalTime(),
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var calendarDto = MapToCalendarDto(appointment, client);
            var eventId = await _googleCalendarService.CreateEventAsync(calendarDto, cancellationToken);
            appointment.GoogleCalendarEventId = eventId;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to sync created appointment to Google Calendar.");
        }

        await _context.Appointments.AddAsync(appointment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(appointment);
    }

    public async Task<AppointmentResponseDto?> RescheduleAsync(Guid id, RescheduleAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (appointment is null) return null;

        appointment.StartTime = dto.NewStartTime.ToUniversalTime();
        appointment.EndTime = dto.NewEndTime.ToUniversalTime();
        appointment.Status = AppointmentStatus.Rescheduled;

        if (!string.IsNullOrEmpty(appointment.GoogleCalendarEventId))
        {
            try
            {
                var calendarDto = MapToCalendarDto(appointment, appointment.Client);
                await _googleCalendarService.UpdateEventAsync(appointment.GoogleCalendarEventId, calendarDto, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update Google Calendar event during rescheduling.");
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return MapToResponseDto(appointment);
    }

    public async Task<AppointmentResponseDto?> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (appointment is null) return null;

        appointment.Title = dto.Title;
        appointment.Notes = dto.Notes;
        appointment.StartTime = dto.StartTime.ToUniversalTime();
        appointment.EndTime = dto.EndTime.ToUniversalTime();
        appointment.Status = dto.Status;

        if (!string.IsNullOrEmpty(appointment.GoogleCalendarEventId))
        {
            try
            {
                var calendarDto = MapToCalendarDto(appointment, appointment.Client);
                await _googleCalendarService.UpdateEventAsync(appointment.GoogleCalendarEventId, calendarDto, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update Google Calendar event.");
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return MapToResponseDto(appointment);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        if (appointment is null) return false;

        if (!string.IsNullOrEmpty(appointment.GoogleCalendarEventId))
        {
            try
            {
                await _googleCalendarService.DeleteEventAsync(appointment.GoogleCalendarEventId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete Google Calendar event.");
            }
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CalendarEventDto MapToCalendarDto(Appointment appointment, Client client) =>
        new(
        Summary: appointment.Title,
        Description: appointment.Notes ?? string.Empty,
        Location: "Consultation Room / Online",
        StartUtc: appointment.StartTime,
        EndUtc: appointment.EndTime,
        ClientEmail: client.Email,
        TimeZone: "UTC"
    );

    private static AppointmentResponseDto MapToResponseDto(Appointment a) =>
        new(
            a.Id,
            a.ClientId,
            a.Client.FullName,
            a.Client.Email,
            a.Title,
            a.Notes,
            a.StartTime,
            a.EndTime,
            a.Status,
            a.GoogleCalendarEventId,
            a.CreatedAt
        );
}