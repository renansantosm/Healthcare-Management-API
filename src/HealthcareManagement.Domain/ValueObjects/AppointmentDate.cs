using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class AppointmentDate : ValueObject<AppointmentDate>
{
    protected AppointmentDate() { }

    private AppointmentDate(DateTimeOffset date)
    {
        Date = date;
    }

    private const int ClinicOpeningHour = 8;
    private const int DefaultAppointmentDurationMinutes = 30;
    public DateTimeOffset Date { get; }

    public static AppointmentDate Create(DateTimeOffset date, IDateTimeProvider dateTimeProvider)
    {
        DomainValidationException.When(date == DateTimeOffset.MinValue, "Invalid appointment date");
        DomainValidationException.When(date < dateTimeProvider.GetUtcNow(), "Appointment date must be in the future");
        DomainValidationException.When(date.Hour < ClinicOpeningHour, "Appointment date must be between 8:00 and 17:00");

        var appontmentDuration = TimeSpan.FromMinutes(DefaultAppointmentDurationMinutes);

        var appointmentEnd = date.Add(appontmentDuration);

        var closingTime = new DateTimeOffset(
            date.Year,
            date.Month,
            date.Day,
            17, 0, 0,
            date.Offset);

        DomainValidationException.When(appointmentEnd > closingTime, "The appointment cannot extend past 17:00");

        return new AppointmentDate(date);
    }

    protected override bool EqualsCore(AppointmentDate other)
    {
        return Date == other.Date;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = 17;
            hashCode = (hashCode * 397) ^ Date.GetHashCode();
            return hashCode;
        }
    }
}
