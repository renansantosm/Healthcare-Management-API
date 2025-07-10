using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class SpecialtyTests
{
    private readonly string _validSpecialty = "Cardiology";
    private readonly string _validSpecialty2 = "Orthopedist";
    private readonly string _invalidSpecialtyMinLength = "ca";
    private readonly string _invalidSpecialtyMaxLength = new string('a', 101);

    [Fact]
    public void CreateSpecialty_WithValidData_ShouldCreateSpecialty()
    {
        var specialty = Specialty.Create(_validSpecialty);

        Assert.NotNull(specialty);
        Assert.Equal("cardiology", specialty.Name);
    }

    [Fact]
    public void CreateSpecialty_WithValidData_ShouldStoreNormalizedName()
    {
        var specialty = Specialty.Create("  Cardiology  ");

        Assert.Equal("cardiology", specialty.Name);
    }

    [Fact]
    public void CreateSpecialty_WithInvalidData_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Specialty.Create(string.Empty));

        Assert.Equal("Specialty name is required", exception.Message);
    }

    [Fact]
    public void CreateSpecialty_WithInvalidSpecialtyMinLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Specialty.Create(_invalidSpecialtyMinLength));

        Assert.Equal("Specialty name too short", exception.Message);
    }

    [Fact]
    public void CreateSpecialty_WithInvalidSpecialtyMaxLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Specialty.Create(_invalidSpecialtyMaxLength));

        Assert.Equal("Specialty name too long", exception.Message);
    }

    [Fact]
    public void Equals_WithSameSpecialty_ShouldReturnTrue()
    {
        var specialty1 = Specialty.Create(_validSpecialty);
        var specialty2 = Specialty.Create(_validSpecialty);

        Assert.True(specialty1.Equals(specialty2));
    }

    [Fact]
    public void Equals_WithDifferentSpecialty_ShouldReturnFalse()
    {
        var specialty1 = Specialty.Create(_validSpecialty);
        var specialty2 = Specialty.Create(_validSpecialty2);

        Assert.False(specialty1.Equals(specialty2));
    }
}
