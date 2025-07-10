using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagement.Infra.Data.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Patient>> GetPatientsAsync(int pageNumber, int pageSize)
    {
        return await _context.Patients.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Guid> CreateAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient.Id;
    }

    public async Task<bool> CpfExistsAsync(string cpf)
    {
        return await _context.Patients.AsNoTracking().AnyAsync(p => p.PersonInfo.Cpf.Number.Equals(cpf));
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Patients.AsNoTracking().AnyAsync(p => p.PersonInfo.Email.Adress.Equals(email));
    }

    public async Task UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Patient patient)
    {
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }
}
