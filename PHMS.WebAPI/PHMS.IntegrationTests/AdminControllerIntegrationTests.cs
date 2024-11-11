using Application.Commands.Administrator;
using Application.DTOs;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace PHMS.IntegrationTests
{
    public class AdminControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;
        private string BaseUrl = "/api/v1/Admin";

        public AdminControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                    services.Remove(descriptor);

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
        public async Task GivenAdmins_WhenGetAllIsCalled_ThenReturnsTheRightContentType()
        {
            // Arrange
            var client = factory.CreateClient();
            SeedAdmins();

            // Act
            var response =client.GetAsync(BaseUrl);

            // Assert
            response.Result.EnsureSuccessStatusCode();
            var admins = response.Result.Content.ReadAsStringAsync().Result;
            admins.Should().NotBeNull();
            admins.Should().Contain("Admin1");
        }

        [Fact]
        public async Task GetById_ShouldReturnAdmin_WhenAdminExists()
        {
            // Arrange
            var client = factory.CreateClient();
            var admin = SeedSingleAdmin();

            // Act
            var response = client.GetAsync($"{BaseUrl}/{admin.Id}");

            // Assert
            response.Result.EnsureSuccessStatusCode();
            var adminDto = await response.Result.Content.ReadFromJsonAsync<AdminDto>();
            adminDto.Should().NotBeNull();
            adminDto.FirstName.Should().Be("Admin1");
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenAdminDoesNotExist()
        {
            // Arrange
            var client = factory.CreateClient();
            var nonExistentId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2881");

            // Act
            var response = await client.GetAsync($"{BaseUrl}/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ShouldModifyAdmin_WhenValidCommandIsGiven()
        {
            // Arrange
            var client = factory.CreateClient();
            var admin = SeedSingleAdmin();

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{admin.Id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
            var updatedAdmin = await dbContext.Admins.FindAsync(admin.Id);
            updatedAdmin.FirstName.Should().Be("UpdatedAdmin");
        }

        [Fact]
        public async Task Delete_ShouldRemoveAdmin_WhenAdminExists()
        {
            // Arrange
            var client = factory.CreateClient();
            var admin = SeedSingleAdmin();

            // Act
            var response = await client.DeleteAsync($"{BaseUrl}/{admin.Id}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
            var deletedAdmin = await dbContext.Admins.FindAsync(admin.Id);
            deletedAdmin.Should().BeNull();
        }

        private void SeedAdmins()
        {
            dbContext.Admins.AddRange(new Admin
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "User1",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Female",
                Email = "admin1@yahoo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1!"),
                PhoneNumber = "0787654321",
                Address = "Address 1"
            },
            new Admin
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin2",
                LastName = "User2",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "admin2@yahoo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin2!"),
                PhoneNumber = "0787654321",
                Address = "Address 2"
            });
            dbContext.SaveChanges();
        }

        private Admin SeedSingleAdmin()
        {
            var admin = new Admin
            {

                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "User1",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Female",
                Email = "admin1@yahoo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1!"),
                PhoneNumber = "0787654321",
                Address = "Address 1"
            };
            dbContext.Admins.Add(admin);
            dbContext.SaveChanges();
            return admin;
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }
    }
}