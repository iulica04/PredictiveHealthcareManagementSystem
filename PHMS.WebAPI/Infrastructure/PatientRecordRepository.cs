using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class PatientRecordRepository : IPatientRecordRepository
    {
        private readonly ApplicationDbContext context;

        public PatientRecordRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(PatientRecord patientRecord)
        {
            try
            {
                await context.PatientRecords.AddAsync(patientRecord);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(patientRecord.PatientRecordId);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var patientRecord = await context.PatientRecords.FindAsync(id);
            if (patientRecord != null)
            {
                context.PatientRecords.Remove(patientRecord);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PatientRecord>> GetAllAsync()
        {
            return await context.PatientRecords
                .Include(p => p.MedicalCondition)
                .ToListAsync();
        }

        public async Task<PatientRecord?> GetByIdAsync(Guid id)
        {
            return await context.PatientRecords.FindAsync(id);
        }

        public async Task UpdateAsync(PatientRecord patientRecord)
        {
            context.Entry(patientRecord).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
