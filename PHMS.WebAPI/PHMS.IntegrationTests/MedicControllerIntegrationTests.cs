using Application.Commands.Medic;
using Domain.Entities;
using FluentAssertions;
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
    public class MedicControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/v1/Medic";

        public MedicControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                        d.ServiceType.FullName?.Contains("Microsoft.EntityFrameworkCore") == true).ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

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
        public async Task GivenMedics_WhenGetAllIsCalled_ThenReturnsAllMedics()
        {
            //Arrange
            var client = factory.CreateClient();

            //Act
            var response = await client.GetAsync(BaseUrl);

            //Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType!.ToString().Should().Be("application/json; charset=utf-8");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GivenExistingMedics_WhenGetAllIsCalled_ThenReturnsAllMedics()
        {
            //Arrange
            var client = factory.CreateClient();
            CreateSUT();

            //Act
            var response = await client.GetAsync(BaseUrl);

            //Assert
            response.EnsureSuccessStatusCode();
            var medics = await response.Content.ReadAsStringAsync();
            medics.Should().Contain("John");
        }

        [Fact]
        public async Task GivenExistingMedicId_WhenGetByIdIsCalled_ThenReturnsTheMedic()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{medicId}");

            //Assert
            response.EnsureSuccessStatusCode();
            var medics = await response.Content.ReadAsStringAsync();
            medics.Should().Contain("Doe");
        }

        [Fact]
        public async Task GivenNonExistingMedicId_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentMedicId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentMedicId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{nonExistentMedicId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenValidMedic_WhenCreateIsCalled_ThenAddToDatabaseTheMedic()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male" // Added required member 'Gender'
            };

            //Act
            await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            var medic = dbContext.Medics.FirstOrDefaultAsync(m => m.FirstName == "John");
            medic.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenMissingFirstName_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicCommand
            {
                FirstName = "",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name cannot be empty.");
        }

        [Fact]
        public async Task GivenFirstNameGreaterThan30Characters_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicCommand
            {
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender="Male"

            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name must be at most 30 characters.");
        }

        [Fact]
        public async Task GivenMissingLastName_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicCommand
            {
                FirstName = "John",
                LastName = "",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name cannot be empty.");
        }

        [Fact]
        public async Task GivenLastNameGreaterThan30Characters_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicCommand
            {
                FirstName = "John",
                LastName = "ChristopherAlexanderJohnsonWilliams",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name must be at most 30 characters.");
        }

        [Fact]
        public async Task GivenExistingMedicId_WhenDeleteIsCalled_ThenMedicIsDeleted()
        {
            // Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{medicId}");
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedMedic = await dbContext.Medics.AsNoTracking().FirstOrDefaultAsync(m => m.Id == medicId);
            deletedMedic.Should().BeNull();
        }

        [Fact]
        public async Task GivenNonExistingMedicId_WhenDeleteIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentMedicId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentMedicId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentMedicId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenExistingMedicId_WhenUpdateIsCalled_ThenMedicIsUpdated()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);

            var command = new UpdateMedicCommand
            {
                Id = medicId,
                FirstName = "John",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var updatedMedic = await dbContext.Medics.AsNoTracking().FirstOrDefaultAsync(m => m.Id == medicId);
            updatedMedic!.FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GivenNonExistingMedicId_WhenUpdateIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentMedicId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentMedicId);

            var command = new UpdateMedicCommand
            {
                Id = nonExistentMedicId,
                FirstName = "John",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentMedicId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenMissingFirstName_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);
            var command = new UpdateMedicCommand
            {
                Id = medicId,
                FirstName = "",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name cannot be empty.");
        }

        [Fact]
        public async Task GivenFirstNameGreaterThan30Characters_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);
            var command = new UpdateMedicCommand
            {
                Id = medicId,
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name must be at most 30 characters.");
        }

        [Fact]
        public async Task GivenMissingLastName_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);
            var command = new UpdateMedicCommand
            {
                Id = medicId,
                FirstName = "John",
                LastName = "",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name cannot be empty.");
        }

        [Fact]
        public async Task GivenLastNameGreaterThan30Characters_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicId = CreateSUTAndReturnMedicId();
            var token = GenerateJwtToken(medicId);
            var command = new UpdateMedicCommand
            {
                Id = medicId,
                FirstName = "John",
                LastName = "ChristopherAlexanderJohnsonWilliams",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                Password = "JohnStrongPass_123",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name must be at most 30 characters.");
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateSUT()
        {
            var medic = new Medic
            {
                FirstName = "John",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                Address = "1234 Main St, Springfield, IL 62701",
                Consultations = new List<Consultation>(),
                Gender = "Male"
            };
            dbContext.Medics.Add(medic);
            dbContext.SaveChanges();
        }

        private Guid CreateSUTAndReturnMedicId()
        {
            var medic = new Medic
            {
                FirstName = "John",
                LastName = "Doe",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "General Hospital",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                PasswordHash = "$2a$11$uR1Iu5Pl3auktlQgyWo3LWWa/xNEeFxuCSsAlaVjG3dB8yLh2x2ji",
                Address = "1234 Main St, Springfield, IL 62701",
                Gender = "Male",
                Consultations = new List<Consultation>()
            };

            dbContext.Medics.Add(medic);
            dbContext.SaveChanges();

            return medic.Id;
        }

        private static string GenerateJwtToken(Guid userId)
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

    }
}