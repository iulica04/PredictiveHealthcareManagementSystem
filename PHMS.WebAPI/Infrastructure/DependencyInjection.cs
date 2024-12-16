using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemoryDatabaseEnvVar)
        {
            if (useInMemoryDatabaseEnvVar)
            {
                services.AddDbContext<ApplicationDbContext>(
                    options => options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(
                    options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
            }

            services.AddScoped<IMedicalConditionRepository, MedicalConditionRepository>();
            services.AddScoped<ITreatmentRepository, TreatmentRepository>();
            services.AddScoped<IMedicationRepository, MedicationRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<IPatientRecordRepository, PatientRecordRepository>();

            return services;
        }
    }
}
