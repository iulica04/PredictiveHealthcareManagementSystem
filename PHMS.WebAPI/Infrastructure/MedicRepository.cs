using Domain.Common;
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

        public async Task<Result<Guid>> AddAsync(Medic medic)
        {
            try
            {
                await context.Medics.AddAsync(medic);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(medic.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var medic = await context.Medics.FirstOrDefaultAsync(x => x.Id == id);
            if (medic != null)
            {
                context.Medics.Remove(medic);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Medic>> GetAllAsync()
        {
            return await context.Medics.ToListAsync();
        }

        public async Task<Medic?> GetByIdAsync(Guid id)
        {
            return await context.Medics.FindAsync(id);
        }

        public Task UpdateAsync(Medic medic)
        {
            context.Medics.Update(medic);
            return context.SaveChangesAsync();

        }
    }
}