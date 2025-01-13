using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPatientRecordRepository
    {
        Task<IEnumerable<PatientRecord>> GetAllAsync();
        Task<PatientRecord?> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(PatientRecord patientRecord);
        Task UpdateAsync(PatientRecord patientRecord);
        Task DeleteAsync(Guid id);
    }
}
