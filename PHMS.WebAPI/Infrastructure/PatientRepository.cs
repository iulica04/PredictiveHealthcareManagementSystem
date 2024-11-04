using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext context;

        public PatientRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Guid> AddAsync(Patient patient)
        {
            await context.Patients.AddAsync(patient);
            await context.SaveChangesAsync();
            return patient.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var pacient = context.Patients.FirstOrDefault(x => x.Id == id);
            if (pacient != null)
            {
                context.Patients.Remove(pacient);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await context.Patients.OfType<Patient>().ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(Guid id)
        {
            return await context.Patients.FindAsync(id);
        }

        public async Task UpdateAsync(Patient pacient)
        {
            context.Entry(pacient).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
