using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Identity.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext context;
        private readonly IConfiguration configuration;

        public UserRepository(UsersDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<Result<Guid>> Register(User user, CancellationToken cancellationToken)
        {
            try
            {
                await context.Users.AddAsync(user, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(user.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result<LoginResponse>> Login(string email, string password)
        {
            var existingUser = await context.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (existingUser is null)
            {
                return Result<LoginResponse>.Failure("Invalid credentials");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, existingUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, existingUser.Type.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Result<LoginResponse>.Success(new LoginResponse
            {
                Token = tokenString,
                Id = existingUser.Id,
                Role = existingUser.Type
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Result<IEnumerable<User>>> GetUsersOfTypeAsync(UserType type)
        {
            var users = await context.Users.Where(u => u.Type == type).ToListAsync();
            if (users is null)
            {
                return Result<IEnumerable<User>>.Failure($"An error occured while fetching users of type {type}");
            }
            return Result<IEnumerable<User>>.Success(users);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await context.Users.FindAsync(id);
        }

        public Task UpdateAsync(User user)
        {
            context.Users.Update(user);
            return context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await context.Users.AnyAsync(x => x.Email == email);
        }
    }
}
