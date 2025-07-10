using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class Cpf : ValueObject<Cpf>
{
    protected Cpf() { }

    private Cpf(string number)
    {
        Number = number;
    }

    public const int CpfLength = 11;
    public string Number { get; }

    public static Cpf Create(string number)
    {
        DomainValidationException.When(string.IsNullOrEmpty(number), "CPF is required");
        DomainValidationException.When(number.Length != CpfLength, "CPF must have exactly 11 digits.");
        DomainValidationException.When(!number.All(char.IsDigit), "CPF must contain only numbers");

        return new Cpf(number);
    }

    protected override bool EqualsCore(Cpf other)
    {
        return Number == other.Number;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = 17;
            hashCode = (hashCode * 317) ^ Number.GetHashCode();
            return hashCode;
        }
    }
}
