namespace Infrastructure.ExternalServices.GoogleCalendar;

using Application.Dtos;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

public class MockGoogleCalendarService : IGoogleCalendarService
{
    private readonly ILogger<MockGoogleCalendarService> _logger;

    public MockGoogleCalendarService(ILogger<MockGoogleCalendarService> logger)
    {
        _logger = logger;
    }

    public Task<string?> CreateEventAsync(CalendarEventDto eventDto, CancellationToken ct = default)
    {
        var mockEventId = $"mock_evt_{Guid.NewGuid():N}";
        _logger.LogInformation(
            "[MOCK CALENDAR] Event Created: '{Summary}' for {ClientEmail} | Start: {StartUtc} | MockId: {MockEventId}",
            eventDto.Summary, eventDto.ClientEmail, eventDto.StartUtc, mockEventId);

        return Task.FromResult<string?>(mockEventId);
    }

    public Task<bool> UpdateEventAsync(string eventId, CalendarEventDto eventDto, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[MOCK CALENDAR] Event Updated: ID '{EventId}' -> '{Summary}' for {ClientEmail}",
            eventId, eventDto.Summary, eventDto.ClientEmail);

        return Task.FromResult(true);
    }

    public Task<bool> DeleteEventAsync(string eventId, CancellationToken ct = default)
    {
        _logger.LogInformation("[MOCK CALENDAR] Event Deleted: ID '{EventId}'", eventId);

        return Task.FromResult(true);
    }
}