namespace Infrastructure.ExternalServices.GoogleCalendar;

using Application.Dtos;
using Application.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly CalendarService _calendarService;
    private readonly GoogleCalendarSettings _settings;
    private readonly ILogger<GoogleCalendarService> _logger;

    public GoogleCalendarService(
        IOptions<GoogleCalendarSettings> options,
        ILogger<GoogleCalendarService> logger)
    {
        _settings = options.Value;
        _logger = logger;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret
            },
            Scopes = [CalendarService.Scope.Calendar]
        });

        var token = new TokenResponse { RefreshToken = _settings.RefreshToken };
        var credential = new UserCredential(flow, "user", token);

        _calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "AppointmentApi"
        });
    }

    public async Task<string?> CreateEventAsync(CalendarEventDto eventDto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating Google Calendar event for {ClientEmail} on {StartUtc}", eventDto.ClientEmail, eventDto.StartUtc);

        var googleEvent = new Event
        {
            Summary = eventDto.Summary,
            Description = eventDto.Description,
            Location = eventDto.Location,
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = eventDto.StartUtc,
                TimeZone = eventDto.TimeZone ?? "UTC"
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = eventDto.EndUtc,
                TimeZone = eventDto.TimeZone ?? "UTC"
            },
            Attendees =
            [
                new() { Email = eventDto.ClientEmail }
            ]
        };

        try
        {
            var request = _calendarService.Events.Insert(googleEvent, _settings.CalendarId);
            request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;

            var createdEvent = await request.ExecuteAsync(ct);
            _logger.LogInformation("Google Calendar event created successfully with ID: {EventId}", createdEvent.Id);

            return createdEvent.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Google Calendar event for {ClientEmail}", eventDto.ClientEmail);
            return null;
        }
    }

    public async Task<bool> UpdateEventAsync(string eventId, CalendarEventDto eventDto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating Google Calendar event ID: {EventId}", eventId);

        var googleEvent = new Event
        {
            Summary = eventDto.Summary,
            Description = eventDto.Description,
            Location = eventDto.Location,
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = eventDto.StartUtc,
                TimeZone = eventDto.TimeZone ?? "UTC"
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = eventDto.EndUtc,
                TimeZone = eventDto.TimeZone ?? "UTC"
            }
        };

        try
        {
            var request = _calendarService.Events.Update(googleEvent, _settings.CalendarId, eventId);
            await request.ExecuteAsync(ct);
            _logger.LogInformation("Successfully updated Google Calendar event ID: {EventId}", eventId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update Google Calendar event ID: {EventId}", eventId);
            return false;
        }
    }

    public async Task<bool> DeleteEventAsync(string eventId, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting Google Calendar event ID: {EventId}", eventId);

        try
        {
            var request = _calendarService.Events.Delete(_settings.CalendarId, eventId);
            await request.ExecuteAsync(ct);
            _logger.LogInformation("Successfully deleted Google Calendar event ID: {EventId}", eventId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Google Calendar event ID: {EventId}", eventId);
            return false;
        }
    }
}