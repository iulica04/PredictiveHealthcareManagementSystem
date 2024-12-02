using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext context;
        public PrescriptionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(Prescription prescription)
        {
            try
            {
                await context.Prescriptions.AddAsync(prescription);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(prescription.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var prescription = await context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                context.Prescriptions.Remove(prescription);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await context.Prescriptions
                .Include(p => p.Medications)
                .ToListAsync();
        }

        public async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await context.Prescriptions.FindAsync(id);
        }

        public Task UpdateAsync(Prescription prescription)
        {
            context.Entry(prescription).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }
    }
}
