using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class PersonInfo : ValueObject<PersonInfo>
{
    protected PersonInfo() { }

    private PersonInfo(FullName fullName, BirthDate birthDate, Cpf cpf, Email email, MobilePhoneNumber mobilePhoneNumber)
    {
        FullName = fullName;
        BirthDate = birthDate;
        Cpf = cpf;
        Email = email;
        MobilePhoneNumber = mobilePhoneNumber;
    }

    public FullName FullName { get; init; }
    public BirthDate BirthDate { get; init; }
    public Cpf Cpf { get; init; }
    public Email Email { get; init;  }
    public MobilePhoneNumber MobilePhoneNumber { get; init; }

    public static PersonInfo Create(FullName fullName, BirthDate birthDate, Cpf cpf, Email email, MobilePhoneNumber mobilePhoneNumber)
    {

        DomainValidationException.When(fullName is null, "Full name is required");
        DomainValidationException.When(birthDate is null, "Birth date is required");
        DomainValidationException.When(cpf is null, "CPF is required");
        DomainValidationException.When(email is null, "Email is required");
        DomainValidationException.When(mobilePhoneNumber is null, "Mobile phone number is required");

        return new PersonInfo(fullName, birthDate, cpf, email, mobilePhoneNumber);
    }

    protected override bool EqualsCore(PersonInfo other)
    {
        return FullName == other.FullName
            && BirthDate == other.BirthDate
            && Cpf == other.Cpf
            && Email == other.Email;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = 17;
            hashCode = (hashCode * 31) ^ FullName.GetHashCode();
            hashCode = (hashCode * 31) ^ Cpf.GetHashCode();
            hashCode = (hashCode * 31) ^ Email.GetHashCode();
            hashCode = (hashCode * 31) ^ MobilePhoneNumber.GetHashCode();
            return hashCode;
        }
    }
}
