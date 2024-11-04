using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class PacientRepository : IPacientRepository
    {
        private readonly ApplicationDbContext context;

        public PacientRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Guid> AddAsync(Patient pacient)
        {
            await context.Patients.AddAsync(pacient);
            await context.SaveChangesAsync();
            return pacient.Id;
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
            return (Patient)await context.Patients.FindAsync(id);
        }

        public async Task UpdateAsync(Patient pacient)
        {
            context.Entry(pacient).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
