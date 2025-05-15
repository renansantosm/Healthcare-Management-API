using HealthcareManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareManagement.Infra.Data.Entities_Configuration;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.OwnsOne(d => d.PersonInfo, personInfo =>
        {
            personInfo.OwnsOne(p => p.FullName, fullName =>
            {
                fullName.Property(f => f.FirstName)
                    .HasColumnName("FirstName")
                    .HasMaxLength(100)
                    .IsRequired();

                fullName.Property(l => l.LastName)
                    .HasColumnName("LastName")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            personInfo.OwnsOne(p => p.BirthDate, birthDate =>
            {
                birthDate.Property(b => b.Date)
                    .HasColumnName("BirthDate")
                    .IsRequired();
            });

            personInfo.OwnsOne(p => p.Cpf, cpf =>
            {
                cpf.Property(c => c.Number)
                    .HasColumnName("Cpf")
                    .HasMaxLength(11)
                    .IsRequired();

                cpf.HasIndex(p => p.Number)
                    .IsUnique()
                    .HasDatabaseName("IX_Doctor_Cpf");
            });

            personInfo.OwnsOne(p => p.Email, email =>
            {
                email.Property(e => e.Adress)
                    .HasColumnName("Email")
                    .HasMaxLength(100)
                    .IsRequired();

                email.HasIndex(p => p.Adress)
                    .IsUnique()
                    .HasDatabaseName("IX_Doctor_Email");
            });

            personInfo.OwnsOne(p => p.MobilePhoneNumber, phone =>
            {
                phone.Property(p => p.Number)
                    .HasColumnName("Phone")
                    .HasMaxLength(11)
                    .IsRequired();
            });
        });

        builder.HasMany(d => d.Appointments)
            .WithOne(a => a.Doctor)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(d => d.Specialties, specialty =>
        {
            specialty.WithOwner()
                .HasForeignKey("DoctorId");

            specialty.Property(s => s.Name)
                .HasColumnName("Specialty")
                .HasMaxLength(100)
                .IsRequired();
        });
    }
}
