using HealthcareManagement.Domain.Interfaces;

namespace HealthcareManagement.Infra.Data.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}
