using Domain.Entities;
using Domain.Common;

namespace Domain.Repositories
{
    public interface IMedicRepository
    {
        Task<IEnumerable<Medic>> GetAllAsync();
        Task<Medic?> GetByIdAsync(Guid id);
        Task<Result<Guid>> AddAsync(Medic medic);
        Task UpdateAsync(Medic medic);
        Task DeleteAsync(Guid id);
        Task<string> Login(string email, string password);
    }
}