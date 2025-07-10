using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareManagement.Infra.Data.Entities_Configuration;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.DoctorId)
            .IsRequired();

        builder.Property(a => a.PatientId)
            .IsRequired();

        builder.OwnsOne(a => a.AppointmentDate, appointmentDate =>
        {
            appointmentDate.Property(a => a.Date)
            .HasColumnName("AppointmentDate")
            .IsRequired();
        });
            
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(a => a.Prescriptions)
            .WithOne(p => p.Appointment)
            .HasForeignKey(p => p.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
