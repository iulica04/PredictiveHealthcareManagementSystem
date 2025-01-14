using Application.Commands.TreatmentCommands;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
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
    public class TreatmentControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/v1/Treatment";

        public TreatmentControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GivenTreatments_WhenGetAllIsCalled_ThenReturnsAllTreatments()
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
        public async Task GivenExistingTreatments_WhenGetAllIsCalled_ThenReturnsAllTreatments()
        {
            //Arrange
            var client = factory.CreateClient();
            CreateSUT();

            //Act
            var response = await client.GetAsync(BaseUrl);

            //Assert
            response.EnsureSuccessStatusCode();
            var treatments = await response.Content.ReadAsStringAsync();
            treatments.Should().Contain("Chemotherapy");
        }

        [Fact]
        public async Task GivenExistingTreatmentId_WhenGetByIdIsCalled_ThenReturnsTheTreatment()
        {
            //Arrange
            var client = factory.CreateClient();
            var treatmentId = CreateSUTAndReturnTreatmentId();
            var token = GenerateJwtToken(treatmentId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{treatmentId}");

            //Assert
            response.EnsureSuccessStatusCode();
            var treatments = await response.Content.ReadAsStringAsync();
            treatments.Should().Contain("Chemotherapy");
        }

        [Fact]
        public async Task GivenNonExistingTreatmentId_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentTreatmentId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentTreatmentId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{nonExistentTreatmentId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenValidTreatment_WhenCreateIsCalled_ThenAddToDatabaseTheTreatment()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateTreatmentCommand
            {
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            var treatment = dbContext.Treatments.FirstOrDefaultAsync(t => t.Name == "Chemotherapy");
            treatment.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenMissingLocation_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateTreatmentCommand
            {
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Location is required.");
        }

        [Fact]
        public async Task GivenInvalidStartDate_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateTreatmentCommand
            {
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("StartDate cannot be in the future.");
        }

        [Fact]
        public async Task GivenInvalidDuration_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateTreatmentCommand
            {
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(-2),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Duration must be after StartDate.");
        }

        [Fact]
        public async Task GivenExistingTreatmentId_WhenDeleteIsCalled_ThenTreatmentIsDeleted()
        {
            // Arrange
            var client = factory.CreateClient();
            var treatmentId = CreateSUTAndReturnTreatmentId();
            var token = GenerateJwtToken(treatmentId);

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{treatmentId}");
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedTreatment = await dbContext.Treatments.AsNoTracking().FirstOrDefaultAsync(t => t.TreatmentId == treatmentId);
            deletedTreatment.Should().BeNull();
        }

        [Fact]
        public async Task GivenNonExistingTreatmentId_WhenDeleteIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentTreatmentId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentTreatmentId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentTreatmentId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenExistingTreatmentId_WhenUpdateIsCalled_ThenTreatmentIsUpdated()
        {
            //Arrange
            var client = factory.CreateClient();
            var treatmentId = CreateSUTAndReturnTreatmentId();
            var token = GenerateJwtToken(treatmentId);

            var command = new UpdateTreatmentCommand
            {
                TreatmentId = treatmentId,
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{treatmentId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var updatedTreatment = await dbContext.Treatments.AsNoTracking().FirstOrDefaultAsync(t => t.TreatmentId == treatmentId);
            updatedTreatment!.Name.Should().Be("Chemotherapy");
        }

        [Fact]
        public async Task GivenNonExistingTreatmentId_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentTreatmentId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentTreatmentId);

            var command = new UpdateTreatmentCommand
            {
                TreatmentId = nonExistentTreatmentId,
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentTreatmentId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenMissingLocation_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var treatmentId = CreateSUTAndReturnTreatmentId();
            var token = GenerateJwtToken(treatmentId);
            var command = new UpdateTreatmentCommand
            {
                TreatmentId = treatmentId,
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{treatmentId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Location is required.");
        }

        [Fact]
        public async Task GivenInvalidStartDate_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var treatmentId = CreateSUTAndReturnTreatmentId();
            var token = GenerateJwtToken(treatmentId);
            var command = new UpdateTreatmentCommand
            {
                TreatmentId = treatmentId,
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{treatmentId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("StartDate cannot be in the future.");
        }

        [Fact]
        public async Task GivenInvalidDuration_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var treatmentId = CreateSUTAndReturnTreatmentId();
            var token = GenerateJwtToken(treatmentId);
            var command = new UpdateTreatmentCommand
            {
                TreatmentId = treatmentId,
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                MedicalConditionId = Guid.NewGuid(),
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(-2),
                Frequency = "Daily",
                Medications = new List<MedicationDto>
                {
                    new MedicationDto
                    {
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{treatmentId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Duration must be after StartDate.");
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateSUT()
        {
            var treatment = new Treatment
            {
                TreatmentId = Guid.NewGuid(),
                MedicalConditionId = Guid.NewGuid(),
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<Medication>
                {
                    new Medication
                    {
                        Id = Guid.NewGuid(),
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };
            dbContext.Treatments.Add(treatment);
            dbContext.SaveChanges();
        }

        private Guid CreateSUTAndReturnTreatmentId()
        {
            var treatment = new Treatment
            {
                TreatmentId = Guid.NewGuid(),
                MedicalConditionId = Guid.NewGuid(),
                Type = TreatmentType.Drug,
                Name = "Chemotherapy",
                Location = "General Hospital",
                StartDate = DateTime.Now.AddDays(-1),
                Duration = DateTime.Now.AddDays(30),
                Frequency = "Daily",
                Medications = new List<Medication>
                {
                    new Medication
                    {
                        Id = Guid.NewGuid(),
                        Name = "Medication1",
                        Type = MedicationType.Tablet,
                        Ingredients = "Ingredient1",
                        AdverseEffects = "None"
                    }
                }
            };

            dbContext.Treatments.Add(treatment);
            dbContext.SaveChanges();

            return treatment.TreatmentId;
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
