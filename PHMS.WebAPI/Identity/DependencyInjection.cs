using Domain.Repositories;
using Identity.Persistence;
using Identity.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration, bool useInMemoryDatabaseEnvVar)
        {
            if (useInMemoryDatabaseEnvVar)
            {
                services.AddDbContext<UsersDbContext>(
                    options => options.UseInMemoryDatabase("TestUsersDb"));
            }
            else
            {
                services.AddDbContext<UsersDbContext>(
                    options => options.UseSqlite(configuration.GetConnectionString("UserConnection")));
            }

            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
