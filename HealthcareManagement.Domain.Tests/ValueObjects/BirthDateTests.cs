using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class BirthDateTests
{
    private readonly DateOnly _validBirthDate = new DateOnly(1900, 01, 01);
    private readonly DateOnly _validBirthDate2 = new DateOnly(1900, 01, 02);
    private readonly DateOnly _invalidBirthDate = new DateOnly(1899, 12, 31);
    private readonly DateOnly _futureDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

    [Fact]
    public void Create_WithValidBirthDate_ShouldCreate()
    {
        var birthDate = BirthDate.Create(_validBirthDate);

        Assert.NotNull(birthDate);
        Assert.Equal(_validBirthDate, birthDate.Date);
    }

    [Fact]
    public void Create_WithInvalidBirthDate_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => BirthDate.Create(_invalidBirthDate));

        Assert.Equal("Birth date cannot be less than 1900-01-01", exception.Message);
    }

    [Fact]
    public void Create_WithFutureBirthDate_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => BirthDate.Create(_futureDate));

        Assert.Equal("Birth date cannot be greater than the current date", exception.Message);
    }

    [Fact]
    public void Equals_WithSameBirthDate_ShouldReturnTrue()
    {
        var birthDate1 = BirthDate.Create(_validBirthDate);
        var birthDate2 = BirthDate.Create(_validBirthDate);

        Assert.True(birthDate1.Equals(birthDate2));
    }

    [Fact]
    public void Equals_WithDifferentBirthDate_ShouldReturnFalse()
    {
        var birthDate1 = BirthDate.Create(_validBirthDate);
        var birthDate2 = BirthDate.Create(_validBirthDate2);

        Assert.False(birthDate1.Equals(birthDate2));
    }
}
