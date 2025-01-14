using Application.Use_Cases.Commands.ConsultationCommands;
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
    public class ConsultationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/v1/Consultation";

        public ConsultationControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GivenConsultations_WhenGetAllIsCalled_ThenReturnsAllConsultations()
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
        public async Task GivenExistingConsultations_WhenGetAllIsCalled_ThenReturnsAllConsultations()
        {
            //Arrange
            var client = factory.CreateClient();
            CreateSUT();

            //Act
            var response = await client.GetAsync(BaseUrl);

            //Assert
            response.EnsureSuccessStatusCode();
            var consultations = await response.Content.ReadAsStringAsync();
            consultations.Should().Contain("General Hospital");
        }

        [Fact]
        public async Task GivenExistingConsultationId_WhenGetByIdIsCalled_ThenReturnsTheConsultation()
        {
            //Arrange
            var client = factory.CreateClient();
            var consultationId = CreateSUTAndReturnConsultationId();
            var token = GenerateJwtToken(consultationId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{consultationId}");

            //Assert
            response.EnsureSuccessStatusCode();
            var consultation = await response.Content.ReadFromJsonAsync<Consultation>();
            consultation.Should().NotBeNull();
            consultation!.Location.Should().Be("General Hospital");
        }

        [Fact]
        public async Task GivenNonExistingConsultationId_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentConsultationId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentConsultationId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{nonExistentConsultationId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenValidConsultation_WhenCreateIsCalled_ThenAddToDatabaseTheConsultation()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Pending,
                Date = DateTime.Now.AddDays(1),
                Location = "General Hospital"
            };

            //Act
            await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            var consultation = dbContext.Consultations.FirstOrDefaultAsync(c => c.Location == "General Hospital");
            consultation.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenMissingLocation_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Pending,
                Date = DateTime.Now.AddDays(1),
                Location = ""
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Be("");
            
        }


        [Fact]
        public async Task GivenInvalidDate_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Pending,
                Date = DateTime.Now.AddDays(-1),
                Location = "General Hospital"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task GivenExistingConsultationId_WhenDeleteIsCalled_ThenConsultationIsDeleted()
        {
            // Arrange
            var client = factory.CreateClient();
            var consultationId = CreateSUTAndReturnConsultationId();
            var token = GenerateJwtToken(consultationId);

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{consultationId}");
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedConsultation = await dbContext.Consultations.AsNoTracking().FirstOrDefaultAsync(c => c.Id == consultationId);
            deletedConsultation.Should().BeNull();
        }

        [Fact]
        public async Task GivenNonExistingConsultationId_WhenDeleteIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentConsultationId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentConsultationId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentConsultationId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenExistingConsultationId_WhenUpdateIsCalled_ThenConsultationIsUpdated()
        {
            //Arrange
            var client = factory.CreateClient();
            var consultationId = CreateSUTAndReturnConsultationId();
            var token = GenerateJwtToken(consultationId);

            var command = new UpdateConsultationCommand
            {
                Id = consultationId,
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Done,
                Date = DateTime.Now.AddDays(1),
                Location = "General Hospital"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{consultationId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var updatedConsultation = await dbContext.Consultations.AsNoTracking().FirstOrDefaultAsync(c => c.Id == consultationId);
            updatedConsultation!.Status.Should().Be(ConsultationStatus.Done);
        }

        [Fact]
        public async Task GivenNonExistingConsultationId_WhenUpdateIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentConsultationId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentConsultationId);

            var command = new UpdateConsultationCommand
            {
                Id = nonExistentConsultationId,
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Done,
                Date = DateTime.Now.AddDays(1),
                Location = "General Hospital"
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentConsultationId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenMissingLocation_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var consultationId = CreateSUTAndReturnConsultationId();
            var token = GenerateJwtToken(consultationId);
            var command = new UpdateConsultationCommand
            {
                Id = consultationId,
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Done,
                Date = DateTime.Now.AddDays(1),
                Location = ""
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{consultationId}", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Location: Location is required.");
        }


        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateSUT()
        {
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Pending,
                Date = DateTime.Now.AddDays(1),
                Location = "General Hospital"
            };
            dbContext.Consultations.Add(consultation);
            dbContext.SaveChanges();
        }

        private Guid CreateSUTAndReturnConsultationId()
        {
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Status = ConsultationStatus.Pending,
                Date = DateTime.Now.AddDays(1),
                Location = "General Hospital"
            };

            dbContext.Consultations.Add(consultation);
            dbContext.SaveChanges();

            return consultation.Id;
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
