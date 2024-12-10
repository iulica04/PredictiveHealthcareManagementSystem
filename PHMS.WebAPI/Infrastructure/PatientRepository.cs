using Domain.Common;
using Domain.Entities;
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
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;

        public PatientRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public async Task<Result<Guid>> AddAsync(Patient patient)
        {
            try
            {
                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(patient.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await context.Patients.ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await context.Patients.FindAsync(id);
        }

        public async Task UpdateAsync(Patient patient)
        {
            context.Entry(patient).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient != null)
            {
                context.Patients.Remove(patient);
                await context.SaveChangesAsync();
            }
        }


        public async Task<LoginResponse> Login(string email, string password)
        {
            var existingPatient = await context.Patients.SingleOrDefaultAsync(x => x.Email == email);
            if (existingPatient == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }



            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, existingPatient.Id.ToString()),
                     new Claim(ClaimTypes.Role,"Patient")
                }),
                Expires = System.DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                Role = "Patient"
            };
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await context.Patients.AnyAsync(p => p.Email == email);
        }

    }
}
