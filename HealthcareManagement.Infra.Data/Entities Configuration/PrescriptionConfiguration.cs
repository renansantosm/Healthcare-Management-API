using HealthcareManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareManagement.Infra.Data.Entities_Configuration;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.AppointmentId)
            .IsRequired();

        builder.Property(p => p.Medication)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Dosage)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Duration)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Instructions)
            .HasMaxLength(500)
            .IsRequired();
    }
}
