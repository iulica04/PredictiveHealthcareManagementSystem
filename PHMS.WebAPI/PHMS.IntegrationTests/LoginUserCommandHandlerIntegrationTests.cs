using Application.Use_Cases.Authentification;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace PHMS.IntegrationTests
{
    public class LoginUserCommandHandlerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;
        private readonly string AdminLoginUrl = "/api/v1/Admin/login";
        private readonly string MedicLoginUrl = "/api/v1/Medic/login";
        private readonly string PatientLoginUrl = "/api/v1/Patient/login";

        public LoginUserCommandHandlerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove all existing registrations for DbContextOptions<ApplicationDbContext>
                    var descriptors = services.Where(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                        d.ServiceType.FullName?.Contains("Microsoft.EntityFrameworkCore") == true).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Add a new provider for InMemoryDatabase
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                });
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenValidMedicCredentials_WhenLoginIsCalled_ThenReturnsToken()
        {
            // Arrange
            var client = factory.CreateClient();
            await SeedMedics();
            var loginCommand = new LoginUserCommand
            {
                Email = "medic1@example.com",
                Password = "Medic1Password!"
            };

            // Act
            var response = await client.PostAsJsonAsync(MedicLoginUrl, loginCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            loginResponse.Should().NotBeNull();
            loginResponse!.Token.Should().NotBeNullOrEmpty();
            loginResponse!.Role.Should().Be("Medic");
        }

        [Fact]
        public async Task GivenValidPatientCredentials_WhenLoginIsCalled_ThenReturnsToken()
        {
            // Arrange
            var client = factory.CreateClient();
            await SeedPatients();
            var loginCommand = new LoginUserCommand
            {
                Email = "patient1@example.com",
                Password = "Patient1Password!"
            };

            // Act
            var response = await client.PostAsJsonAsync(PatientLoginUrl, loginCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            loginResponse.Should().NotBeNull();
            loginResponse!.Token.Should().NotBeNullOrEmpty();
            loginResponse!.Role.Should().Be("Patient");
        }

        [Fact]
        public async Task GivenValidAdminCredentials_WhenLoginIsCalled_ThenReturnsToken()
        {
            // Arrange
            var client = factory.CreateClient();
            await SeedAdmins();
            var loginCommand = new LoginUserCommand
            {
                Email = "admin1@example.com",
                Password = "Admin1Password!"
            };

            // Act
            var response = await client.PostAsJsonAsync(AdminLoginUrl, loginCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            loginResponse.Should().NotBeNull();
            loginResponse!.Token.Should().NotBeNullOrEmpty();
            loginResponse!.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task GivenInvalidCredentials_WhenLoginIsCalled_ThenReturnsUnauthorized()
        {
            // Arrange
            var client = factory.CreateClient();
            var loginCommand = new LoginUserCommand
            {
                Email = "invalid@example.com",
                Password = "InvalidPassword!"
            };

            // Act
            var response = await client.PostAsJsonAsync(AdminLoginUrl, loginCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid credentials");
        }

        [Fact]
        public async Task GivenCorrectEmailButIncorrectPassword_WhenLoginIsCalled_ThenReturnsUnauthorized()
        {
            // Arrange
            var client = factory.CreateClient();
            await SeedAdmins();
            var loginCommand = new LoginUserCommand
            {
                Email = "admin1@example.com",
                Password = "WrongPassword!"
            };

            // Act
            var response = await client.PostAsJsonAsync(AdminLoginUrl, loginCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid credentials");
        }

        private async Task SeedMedics()
        {
            var medic1 = new Medic
            {
                Id = Guid.NewGuid(),
                FirstName = "Medic1",
                LastName = "User1",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "medic1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Medic1Password!"),
                PhoneNumber = "0787654321",
                Address = "Address 1",
                Rank = "Captain",
                Specialization = "Cardiology",
                Hospital = "General Hospital"
            };
            await dbContext.Medics.AddAsync(medic1);
            await dbContext.SaveChangesAsync();
        }

        private async Task SeedPatients()
        {
            var patient1 = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = "Patient1",
                LastName = "User1",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Female",
                Email = "patient1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient1Password!"),
                PhoneNumber = "0787654321",
                Address = "Address 1",
                MedicalConditions = new List<MedicalCondition>()
            };
            await dbContext.Patients.AddAsync(patient1);
            await dbContext.SaveChangesAsync();
        }

        private async Task SeedAdmins()
        {
            var admin1 = new Admin
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin1",
                LastName = "User1",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "admin1@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1Password!"),
                PhoneNumber = "0787654321",
                Address = "Address 1"
            };
            await dbContext.Admins.AddAsync(admin1);
            await dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

