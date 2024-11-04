using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class MedicRepository : IMedicRepository
    {
        private readonly ApplicationDbContext context;

        public MedicRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Guid> AddAsync(Medic medic)
        {
            await context.Medics.AddAsync(medic);
            await context.SaveChangesAsync();
            return medic.Id;
        }

        public async Task DeleteAsync(Guid id)
        {
            var medic = context.Medics.FirstOrDefault(x => x.Id == id);
            if (medic != null)
            {
                context.Medics.Remove(medic);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medic>> GetAllAsync()
        {
            return await context.Medics.OfType<Medic>().ToListAsync();
        }

        public Task<Medic> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Medic medic)
        {
            context.Medics.Update(medic);
            return context.SaveChangesAsync();

        }
    }
}