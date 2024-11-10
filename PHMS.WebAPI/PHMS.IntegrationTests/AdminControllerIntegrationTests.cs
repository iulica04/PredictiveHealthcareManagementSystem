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
    /*public class AdminControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;
        private string BaseUrl = "/api/v1/admin";

        public AdminControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
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
        public async Task GetAll_ShouldReturnAdmins()
        {
            // Arrange
            var client = factory.CreateClient();
            SeedAdmins();

            // Act
            var response = await client.GetAsync(BaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var admins = await response.Content.ReadFromJsonAsync<List<AdminDto>>();
            admins.Should().NotBeNull();
            admins.Should().Contain(a => a.FirstName == "Admin1");
        }

        [Fact]
        public async Task GetById_ShouldReturnAdmin_WhenAdminExists()
        {
            // Arrange
            var client = factory.CreateClient();
            var admin = SeedSingleAdmin();

            // Act
            var response = await client.GetAsync($"{BaseUrl}/{admin.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var adminDto = await response.Content.ReadFromJsonAsync<AdminDto>();
            adminDto.Should().NotBeNull();
            adminDto.FirstName.Should().Be("Admin1");
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenAdminDoesNotExist()
        {
            // Arrange
            var client = factory.CreateClient();
            var nonExistentId = Guid.NewGuid();

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
                Id = admin.Id,
                FirstName = "UpdatedAdmin",
                LastName = "UpdatedLastName",
                Email = "updated@example.com",
                PhoneNumber = "9999999999",
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
                Id = Guid.NewGuid(),
                FirstName = "Admin1",
                LastName = "User1",
                Email = "admin1@example.com",
                PhoneNumber = "1234567890",
                Address = "Address1"
            },
            new Admin
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin2",
                LastName = "User2",
                Email = "admin2@example.com",
                PhoneNumber = "0987654321",
                Address = "Address2"
            });
            dbContext.SaveChanges();
        }

        private Admin SeedSingleAdmin()
        {
            var admin = new Admin
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin1",
                LastName = "User1",
                Email = "admin1@example.com",
                PhoneNumber = "1234567890",
                Address = "Address1"
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
    }*/
}