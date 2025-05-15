using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class BirthDate : ValueObject<BirthDate>
{
    protected BirthDate() { }

    private BirthDate(DateOnly date)
    {
        Date = date;
    }

    private static readonly DateOnly _minBirthDate = new DateOnly(1900, 01, 01);
    public DateOnly Date { get; }

    public static BirthDate Create(DateOnly date)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        DomainValidationException.When(date < _minBirthDate, "Birth date cannot be less than 1900-01-01");
        DomainValidationException.When(date > currentDate, "Birth date cannot be greater than the current date");

        return new BirthDate(date);
    }

    protected override bool EqualsCore(BirthDate other)
    {
        return Date == other.Date;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = 17;
            hashCode = (hashCode * 317) ^ Date.GetHashCode();
            return hashCode;
        }
    }
}
