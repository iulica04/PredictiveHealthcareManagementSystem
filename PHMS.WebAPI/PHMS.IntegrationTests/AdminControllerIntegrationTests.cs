using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Identity.Persistence;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

namespace PHMS.IntegrationTests
{
    public class AdminControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly UsersDbContext dbContext;
        private string BaseUrl = "/api/v1/Admin";

        public AdminControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Elimină toate înregistrările pentru DbContextOptions<ApplicationDbContext>
                    var descriptors = services.Where(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                        d.ServiceType.FullName?.Contains("Microsoft.EntityFrameworkCore") == true).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Adaugă un nou provider pentru InMemoryDatabase
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                });
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenAdmins_WhenGetAllIsCalled_ThenReturnsTheRightContentType()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(BaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GetById_ShouldReturnAdmin_WhenAdminExists()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();

            // Act
            var response = await client.GetAsync($"{BaseUrl}/{adminId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var adminDto = await response.Content.ReadAsStringAsync();
            adminDto.Should().NotBeNull();
            adminDto.Should().Contain("Admin1");
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
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        /*
        [Fact]
        public async Task GivenValidAdmin_WhenUpdateIsCalled_Then_ShouldUpdateTheAdminInTheDatabase()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = adminId,
                FirstName = "Update Admin1",
                LastName = "UpdatedLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var updatedAdmin = await dbContext.Users.FirstOrDefaultAsync(a => a.Id == adminId);
            updatedAdmin.Should().NotBeNull();
            updatedAdmin!.FirstName.Should().Be("Update Admin1");
        }

        [Fact]
        public async Task GivenNonExistingAdminId_WhenUpdateIsCalled_ThenReturnsNotFound()
        {
            // Arrange
            var client = factory.CreateClient();
            var nonExistentAdminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2882");
            SeedAdmins();
            var token = GenerateJwtToken(nonExistentAdminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = nonExistentAdminId,
                FirstName = "Update Admin1",
                LastName = "UpdatedLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentAdminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task GivenMissingFirstName_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "",
                LastName = "UpdatedLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name cannot be empty.");
        }
        
        [Fact]
        public async Task GivenFirstNameGreaterThan30Characters_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "AdministratorAdministratorAdministrator",
                LastName = "UpdatedLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name must be at most 30 characters.");
        }

        [Fact]
        public async Task GivenMissingLastName_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name cannot be empty.");
        }

        [Fact]
        public async Task GivenLastNameGreaterThan30Characters_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "LastNameGreaterThan30Characters",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name must be at most 30 characters.");
        }

        [Fact]
        public async Task GivenInvalidBirthDate_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "LastNameGreaterThan30Characters",
                BirthDate = DateTime.Now.AddYears(2),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Birthday must be in the past.");
        }

        [Fact]
        public async Task GivenInvalidGender_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "LastName",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "invalidGender",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Gender must be either 'Male' or 'Female'.");
        }

        [Fact]
        public async Task GivenInvalidEmail_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "LastName",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "updated.email.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid email format.");
        }

        [Fact]
        public async Task GivenInvalidPhoneNumber_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            var updateCommand = new UpdateAdminCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Admin1",
                LastName = "LastName",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "updated.email@yahoo.com",
                Password = "newPassword123!",
                PhoneNumber = "invalid0787654321",
                Address = "Updated Address"
            };

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{adminId}", updateCommand);
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid phone number format.");
        }*/


        [Fact]
        public async Task GivenExistingAdminId_WhenDeleteIsCalled_ThenAdminIsDeleted()
        {
            // Arrange
            var client = factory.CreateClient();
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888");
            SeedAdmins();
            var token = GenerateJwtToken(adminId);

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{adminId}");
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedAdmin = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Id == adminId);
            deletedAdmin.Should().BeNull();
        }
        [Fact]
        public async Task GivenNonExistingAdminId_WhenDeleteIsCalled_ThenReturnsNotFound()
        {
            // Arrange
            var client = factory.CreateClient();
            var nonExistentId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2881");
            SeedAdmins();
            var token = GenerateJwtToken(nonExistentId);

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentId}");
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }


        private void SeedAdmins()
        {
            var admin1 = new Admin
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Admin,
                FirstName = "Admin1",
                LastName = "User1",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Female",
                Email = "admin1@yahoo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1!"),
                PhoneNumber = "0787654321",
                Address = "Address 1"
            };
            var admin2 = new Admin
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888"),
                Type = UserType.Admin,
                FirstName = "Admin2",
                LastName = "User2",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "admin2@yahoo.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin2!"),
                PhoneNumber = "0787654321",
                Address = "Address 2"
            };
            dbContext.Users.Add(admin1);
            dbContext.Users.Add(admin2);
            dbContext.SaveChanges();
        }

        private string GenerateJwtToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("My Secret Key For Identity Module");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, userId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}