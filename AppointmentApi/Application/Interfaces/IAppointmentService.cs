namespace Application.Interfaces;

using Application.Dtos;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponseDto>> GetUpcomingAppointmentsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<AppointmentResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentResponseDto?> RescheduleAsync(Guid id, RescheduleAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<AppointmentResponseDto?> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}