namespace Application.Dtos;

public record CalendarEventDto(
    string Summary,
    string Description,
    string ClientEmail,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Location = null,
    string? TimeZone = "UTC"
);