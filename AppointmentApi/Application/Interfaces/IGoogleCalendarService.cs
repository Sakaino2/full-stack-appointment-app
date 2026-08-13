namespace Application.Interfaces;

using Domain.Entities;

public interface IGoogleCalendarService
{
    Task<string?> CreateEventAsync(Appointment appointment, Client client, CancellationToken cancellationToken = default);
    Task UpdateEventAsync(Appointment appointment, Client client, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(string googleCalendarEventId, CancellationToken cancellationToken = default);
}