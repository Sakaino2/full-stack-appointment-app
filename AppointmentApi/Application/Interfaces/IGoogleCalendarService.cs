namespace Application.Interfaces;

using Application.Dtos;

public interface IGoogleCalendarService
{
    Task<string?> CreateEventAsync(CalendarEventDto eventDto, CancellationToken ct = default);
    Task<bool> UpdateEventAsync(string eventId, CalendarEventDto eventDto, CancellationToken ct = default);
    Task<bool> DeleteEventAsync(string eventId, CancellationToken ct = default);
}