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
        public async Task<Guid> AddAsync(Pacient pacient)
        {
            await context.Pacients.AddAsync(pacient);
            await context.SaveChangesAsync();
            return pacient.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var pacient = context.Pacients.FirstOrDefault(x => x.Id == id);
            if (pacient != null)
            {
                context.Pacients.Remove(pacient);
                await context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<Pacient>> GetAllAsync()
        {
            return await context.Pacients.OfType<Pacient>().ToListAsync();
        }

        public async Task<Pacient> GetByIdAsync(Guid id)
        {
            return (Pacient)await context.Pacients.FindAsync(id);
        }

        public async Task UpdateAsync(Pacient pacient)
        {
            context.Entry(pacient).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
