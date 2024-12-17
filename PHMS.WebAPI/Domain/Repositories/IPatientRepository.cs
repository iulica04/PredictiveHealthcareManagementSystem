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
        Task<LoginResponse> Login(string email, string password);
        Task<bool> ExistsByEmailAsync(string email);
        Task<Patient?> GetByEmailAsync(string email);

    }
}
