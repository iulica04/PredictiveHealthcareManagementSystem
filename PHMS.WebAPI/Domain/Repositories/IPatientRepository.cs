using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Patient pacient);
        Task UpdateAsync(Patient pacient);
        Task DeleteAsync(Guid id);
    }
}
