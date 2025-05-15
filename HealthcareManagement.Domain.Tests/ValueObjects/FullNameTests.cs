using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class FullNameTests
{
    private readonly string _firstName = "John";
    private readonly string _fistName2 = "Jane";
    private readonly string _lastName = "Doe";
    private readonly string _invalidNameMinLength = "Jo";
    private readonly string _invalidNameMaxLength = new string('a', 101);

    [Fact]
    public void CreateFullName_WithValidData_ShouldCreateFullName()
    {
        var fullName = FullName.Create(_firstName, _lastName);

        Assert.NotNull(fullName);
        Assert.Equal(_firstName, fullName.FirstName);
    }

    [Fact]
    public void CreateFullName_WithInvalidFirstName_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(string.Empty, _lastName));

        Assert.Equal("First name is required", exception.Message);
    }

    [Fact]
    public void CreateFullName_WithInvalidLastName_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(_firstName, string.Empty));

        Assert.Equal("Last name is required", exception.Message);
    }

    [Fact]
    public void CreateFullName_WithInvalidData_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(string.Empty, string.Empty));

        Assert.Equal("First name is required", exception.Message);
    }

    [Fact]
    public void CreateFullName_WithInvalidFirstNameLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(_invalidNameMinLength, _lastName));

        Assert.Equal("Name must have at least 3 characters", exception.Message);
    }

    [Fact]
    public void CreateFullName_WithInvalidLastNameLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(_firstName, _invalidNameMinLength));

        Assert.Equal("Last name must have at least 3 characters", exception.Message);
    }

    [Fact]
    public void CreateFullName_WithInvalidFirstNameMaxLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(_invalidNameMaxLength, _lastName));

        Assert.Equal("Name must have a maximum of 100 characters", exception.Message);
    }

    [Fact]
    public void CreateFullName_WithInvalidLastNameMaxLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => FullName.Create(_firstName, _invalidNameMaxLength));

        Assert.Equal("Last name must have a maximum of 100 characters", exception.Message);
    }

    [Fact]
    public void Equals_WithSameFullName_ShouldReturnTrue()
    {
        var fullName1 = FullName.Create(_firstName, _lastName);
        var fullName2 = FullName.Create(_firstName, _lastName);

        Assert.True(fullName1.Equals(fullName2));
    }

    [Fact]
    public void Equals_WithDifferentFullName_ShouldReturnFalse()
    {
        var fullName1 = FullName.Create(_firstName, _lastName);
        var fullName2 = FullName.Create(_fistName2, _lastName);

        Assert.False(fullName1.Equals(fullName2));
    }
}