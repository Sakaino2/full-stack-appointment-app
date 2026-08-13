using Domain.Entities;

namespace Application.Dtos;

public record CreateAppointmentDto(
    Guid ClientId,
    string Title,
    string? Notes,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc
);

public record RescheduleAppointmentDto(
    DateTime NewStartTimeUtc,
    DateTime NewEndTimeUtc
);

public record UpdateAppointmentDto(
    string Title,
    string? Notes,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    AppointmentStatus Status
);

public record AppointmentResponseDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string ClientEmail,
    string Title,
    string? Notes,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    AppointmentStatus Status,
    string? GoogleCalendarEventId,
    DateTime CreatedAt
);