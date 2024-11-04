using Domain.Common;
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
        public async Task<Result<Guid>> AddAsync(Patient patient)
        {
            try
            {
                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(patient.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await context.Patients.ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(Guid id)
        {
            return await context.Patients.FindAsync(id);
        }

        public async Task UpdateAsync(Patient patient)
        {
            context.Entry(patient).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        //Delete
        public async Task DeleteAsync(Guid id)
        {
            var patient = context.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with Id {id} not found.");
            }
            // context.Patients.Remove(patient);
            await context.SaveChangesAsync();
        }
    }
}
