﻿using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;

        public AdminRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await context.Admins.ToListAsync();
        }
        public async Task<Admin?> GetByIdAsync(Guid id)
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
        public async Task<string> Login(string email, string password)
        {
            var existingAdmin = await context.Admins.SingleOrDefaultAsync(x => x.Email == email);
            if (existingAdmin == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, existingAdmin.Id.ToString())
                }),
                Expires = System.DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
