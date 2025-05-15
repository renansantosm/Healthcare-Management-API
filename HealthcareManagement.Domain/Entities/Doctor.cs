using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Entities;

public sealed class Doctor : Entity
{
    protected Doctor() { }

    public Doctor(Guid id, PersonInfo info) : base(id)
    {
        SetDomainProperties(info);
    }

    private const int MaxSpecialtiesAllowed = 2;
    private const int AppointmentDuration = 30;
    private readonly IList<Specialty> _specialties = new List<Specialty>();
    private readonly IList<Appointment> _appointments = new List<Appointment>();
    public PersonInfo PersonInfo { get; private set; }
    public IReadOnlyCollection<Specialty> Specialties => _specialties.AsReadOnly();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public void Update(PersonInfo info)
    {
        SetDomainProperties(info);
    }

    public void AddSpecialty(Specialty specialty)
    {
        DomainValidationException.When(Specialties.Count >= MaxSpecialtiesAllowed, "A doctor cannot have more than two specialties");

        var existingSpecialty = _specialties.FirstOrDefault(s => s.Equals(specialty));
        DomainValidationException.When(existingSpecialty is not null, "This specialty is already registered for this doctor.");

        _specialties.Add(specialty);
    }

    public void RemoveSpecialty(Specialty specialty)
    {
        var existingSpecialty = _specialties.FirstOrDefault(s => s.Equals(specialty));
        DomainValidationException.When(existingSpecialty is null, "Specialty not found for this doctor.");

        _specialties.Remove(existingSpecialty);
    }

    private void SetDomainProperties(PersonInfo info)
    {
        PersonInfo = info;
    }
}
