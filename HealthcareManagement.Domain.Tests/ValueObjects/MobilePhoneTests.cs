using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class MobilePhoneTests
{
    private readonly string _validMobilePhone = "35987654321";
    private readonly string _validMobilePhone2 = "35987654320";
    private readonly string _invalidMobilePhoneNotStartingWith9 = "35897654321";
    private readonly string _invalidMobilePhoneInvalidAreaCode = "09987654321";
    private readonly string _invalidMobilePhoneLength = "359876";

    [Fact]
    public void CreateMobilePhone_WithValidData_ShouldCreateMobilePhone()
    {
        var mobilePhone = MobilePhoneNumber.Create(_validMobilePhone);

        Assert.NotNull(mobilePhone);
        Assert.Equal(_validMobilePhone, mobilePhone.Number);
    }

    [Fact]
    public void CreateMobilePhone_WithInvalidData_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => MobilePhoneNumber.Create(string.Empty));

        Assert.Equal("Mobile phone number is required", exception.Message);
    }

    [Fact]
    public void CreateMobilePhone_WithMobilePhoneNotStartingWith9_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => MobilePhoneNumber.Create(_invalidMobilePhoneNotStartingWith9));

        Assert.Equal("Invalid mobile phone number format", exception.Message);
    }

    [Fact]
    public void CreateMobilePhone_WithInvalidMobilePhoneAreaCode_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => MobilePhoneNumber.Create(_invalidMobilePhoneInvalidAreaCode));

        Assert.Equal("Invalid mobile phone number format", exception.Message);
    }

    [Fact]
    public void CreateMobilePhone_WithInvalidMobilePhoneLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => MobilePhoneNumber.Create(_invalidMobilePhoneLength));

        Assert.Equal("Invalid mobile phone number format", exception.Message);
    }

    [Fact]
    public void Equals_WithSameMobilePhone_ShouldReturnTrue()
    {
        var mobilePhone1 = MobilePhoneNumber.Create(_validMobilePhone);
        var mobilePhone2 = MobilePhoneNumber.Create(_validMobilePhone);

        Assert.True(mobilePhone1.Equals(mobilePhone2));
    }

    [Fact]
    public void Equals_WithDifferentMobilePhone_ShouldReturnFalse()
    {
        var mobilePhone1 = MobilePhoneNumber.Create(_validMobilePhone);
        var mobilePhone2 = MobilePhoneNumber.Create(_validMobilePhone2);

        Assert.False(mobilePhone1.Equals(mobilePhone2));
    }
}
