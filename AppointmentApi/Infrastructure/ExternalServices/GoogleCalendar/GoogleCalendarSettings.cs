namespace Infrastructure.ExternalServices.GoogleCalendar;

public class GoogleCalendarSettings
{
    public const string SectionName = "GoogleCalendarSettings";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string CalendarId { get; set; } = "primary";
}