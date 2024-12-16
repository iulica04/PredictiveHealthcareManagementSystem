using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> Register(User user, CancellationToken cancellationToken);
        Task<Result<LoginResponse>> Login(string email, string passwordHash);
        Task<Result<IEnumerable<User>>> GetUsersOfTypeAsync(UserType type);
        Task<Result<User?>> GetUserByIdAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
