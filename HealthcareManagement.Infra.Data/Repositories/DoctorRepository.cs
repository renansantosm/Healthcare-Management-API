using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagement.Infra.Data.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly AppDbContext _context;

    public DoctorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsAsync(int pageNumber, int pageSize)
    {
        return await _context.Doctors.Include(d => d.Specialties)
            .AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<Doctor?> GetByIdAsync(Guid id)
    {
        return await _context.Doctors.Include(d => d.Specialties).FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Guid> CreateAsync(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return doctor.Id;
    }

    public async Task<bool> CpfExistsAsync(string cpf)
    {
        return await _context.Doctors.AsNoTracking()
            .AnyAsync(d => d.PersonInfo.Cpf.Number.Equals(cpf));
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Doctors.AsNoTracking()
            .AnyAsync(d => d.PersonInfo.Email.Adress.Equals(email));
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Doctor doctor)
    {
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
    }
}
