using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPacientRepository
    {
        Task<IEnumerable<Pacient>> GetAllAsync();
        Task<Pacient> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(Pacient pacient);
        Task UpdateAsync(Pacient pacient);
        Task DeleteAsync(Guid id);
    }
}
