using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Guid id);
        Task<string> Login(string email, string password);
    }
}
