using HealthcareManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareManagement.Infra.Data.Entities_Configuration;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

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
                    .HasDatabaseName("IX_Patient_Cpf");
            });

            personInfo.OwnsOne(p => p.Email, email =>
            {
                email.Property(e => e.Adress)
                    .HasColumnName("Email")
                    .HasMaxLength(100)
                    .IsRequired();

                email.HasIndex(p => p.Adress)
                    .IsUnique()
                    .HasDatabaseName("IX_Patient_Email");
            });

            personInfo.OwnsOne(p => p.MobilePhoneNumber, phone =>
            {
                phone.Property(p => p.Number)
                    .HasColumnName("Phone")
                    .HasMaxLength(11)
                    .IsRequired();
            });
        });

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
