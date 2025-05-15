using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;
using System.Text.RegularExpressions;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class MobilePhoneNumber : ValueObject<MobilePhoneNumber>
{
    protected MobilePhoneNumber() { }

    private MobilePhoneNumber(string number)
    {
        Number = number;
    }

    private static readonly Regex PhoneNumberRegex = new(@"^(1[1-9]|[4689][0-9]|2[12478]|3([1-5]|[7-8])|5([13-5])|7[193-7])9[0-9]{8}$");
    public string Number { get; }

    public static MobilePhoneNumber Create(string number)
    {
        DomainValidationException.When(string.IsNullOrEmpty(number), "Mobile phone number is required");
        DomainValidationException.When(!PhoneNumberRegex.IsMatch(number), "Invalid mobile phone number format");

        return new MobilePhoneNumber(number);
    }

    protected override bool EqualsCore(MobilePhoneNumber other)
    {
        return Number == other.Number;
    }

    protected override int GetHashCodeCore()
    {
        int hashCode = 17;
        hashCode = (hashCode * 317) ^ Number.GetHashCode();
        return hashCode;
    }
}
