namespace Domain.Entities;

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled,
    Rescheduled
}

public class Appointment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public string? GoogleCalendarEventId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}