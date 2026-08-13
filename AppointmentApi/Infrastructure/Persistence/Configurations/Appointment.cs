using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentApi.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        builder.Property(a => a.Status)
            .HasConversion<string>() // Stores enum values as string ("Scheduled", "Completed", etc.)
            .HasMaxLength(20);

        builder.Property(a => a.GoogleCalendarEventId)
            .HasMaxLength(250);

        // One-To-Many Relationship Setup
        builder.HasOne(a => a.Client)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}