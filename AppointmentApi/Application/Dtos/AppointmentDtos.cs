using Domain.Entities;

namespace Application.Dtos;

public record CreateAppointmentDto(
    Guid ClientId,
    string Title,
    string? Notes,
    DateTime StartTime,
    DateTime EndTime
);

public record RescheduleAppointmentDto(
    DateTime NewStartTime,
    DateTime NewEndTime
);

public record UpdateAppointmentDto(
    string Title,
    string? Notes,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status
);

public record AppointmentResponseDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string ClientEmail,
    string Title,
    string? Notes,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status,
    string? GoogleCalendarEventId,
    DateTime CreatedAt
);