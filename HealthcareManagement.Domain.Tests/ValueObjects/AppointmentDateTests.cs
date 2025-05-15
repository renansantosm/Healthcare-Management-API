using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class AppointmentDateTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    [Fact]
    public void Create_WithValidDate_ShouldCreate()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date = new DateTimeOffset(2025, 1, 2, 8, 0, 0, TimeSpan.Zero);

        var appointmentDate = AppointmentDate.Create(date, _dateTimeProvider);

        Assert.NotNull(appointmentDate);
        Assert.Equal(date, appointmentDate.Date);
    }

    [Fact]
    public void Create_WithPastDate_ShouldThrowDomainValidationException()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date = new DateTimeOffset(2024, 12, 31, 8, 0, 0, TimeSpan.Zero);

        var exception = Assert.Throws<DomainValidationException>(() => AppointmentDate.Create(date, _dateTimeProvider));

        Assert.Equal("Appointment date must be in the future", exception.Message);
    }

    [Fact]
    public void Create_WithAppointmentBeforeClinicOpening_ShouldThrowDomainValidationException()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date = new DateTimeOffset(2025, 1, 2, 7, 59, 0, TimeSpan.Zero);

        var exception = Assert.Throws<DomainValidationException>(() => AppointmentDate.Create(date, _dateTimeProvider));

        Assert.Equal("Appointment date must be between 8:00 and 17:00", exception.Message);
    }

    [Fact] 
    public void Create_WithAppointmentAfterClinicClosing_ShouldThrowDomainValidationException()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date = new DateTimeOffset(2025, 1, 2, 17, 1, 0, TimeSpan.Zero);

        var exception = Assert.Throws<DomainValidationException>(() => AppointmentDate.Create(date, _dateTimeProvider));

        Assert.Equal("The appointment cannot extend past 17:00", exception.Message);
    }

    [Fact]
    public void Create_WithInvalidDate_ShouldThrowDomainValidationException()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date = DateTimeOffset.MinValue;

        var exception = Assert.Throws<DomainValidationException>(() => AppointmentDate.Create(date, _dateTimeProvider));

        Assert.Equal("Invalid appointment date", exception.Message);
    }

    [Fact]
    public void Equals_WithSameDate_ShouldReturnTrue()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date = new DateTimeOffset(2025, 1, 2, 8, 0, 0, TimeSpan.Zero);

        var appointmentDate1 = AppointmentDate.Create(date, _dateTimeProvider);
        var appointmentDate2 = AppointmentDate.Create(date, _dateTimeProvider);

        Assert.True(appointmentDate1.Equals(appointmentDate2));
    }

    [Fact]
    public void Equals_WithDifferentDate_ShouldReturnFalse()
    {
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));
        var date1 = new DateTimeOffset(2025, 1, 2, 8, 0, 0, TimeSpan.Zero);
        var date2 = new DateTimeOffset(2025, 1, 3, 8, 0, 0, TimeSpan.Zero);

        var appointmentDate1 = AppointmentDate.Create(date1, _dateTimeProvider);
        var appointmentDate2 = AppointmentDate.Create(date2, _dateTimeProvider);

        Assert.False(appointmentDate1.Equals(appointmentDate2));
    }
}
