using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> GetAllAsync();
        Task<Prescription?> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Prescription prescription);
        Task UpdateAsync(Prescription prescription);
        Task DeleteAsync(Guid id);
    }
}
