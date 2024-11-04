using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPacientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(Patient pacient);
        Task UpdateAsync(Patient pacient);
        Task DeleteAsync(Guid id);
    }
}
