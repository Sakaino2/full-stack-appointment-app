using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class MockGoogleCalendarService : IGoogleCalendarService
{
    private readonly ILogger<MockGoogleCalendarService> _logger;

    public MockGoogleCalendarService(ILogger<MockGoogleCalendarService> logger)
    {
        _logger = logger;
    }

    public Task<string?> CreateEventAsync(Appointment appointment, Client client, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK] Google Calendar Create skipped for Appointment ID: {Id}", appointment.Id);
        return Task.FromResult<string?>("mock-event-id-123");
    }

    public Task UpdateEventAsync(Appointment appointment, Client client, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK] Google Calendar Update skipped for Appointment ID: {Id}", appointment.Id);
        return Task.CompletedTask;
    }

    public Task DeleteEventAsync(string eventId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK] Google Calendar Delete skipped for Event ID: {EventId}", eventId);
        return Task.CompletedTask;
    }
}