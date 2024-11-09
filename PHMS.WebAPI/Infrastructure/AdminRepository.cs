using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext context;
        public AdminRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await context.Admins.ToListAsync();
        }
        public async Task<Admin> GetByIdAsync(Guid id)
        {
            return await context.Admins.FindAsync(id);
        }
        public async Task UpdateAsync(Admin admin)
        {
            context.Entry(admin).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var admin = await context.Admins.FindAsync(id);
            if (admin != null)
            {
                context.Admins.Remove(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
