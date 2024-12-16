using Domain.Entities;

namespace Domain.Repositories
{
    public interface IMedicRepository
    {
        Task<IEnumerable<Medic>> GetAllAsync();
        Task<Medic?> GetByIdAsync(Guid id);
        Task UpdateAsync(Medic medic);
        Task DeleteAsync(Guid id);
    }
}