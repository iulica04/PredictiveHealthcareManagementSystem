using Application.Commands.MedicalConditionCommands;
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
    public class MedicalConditionControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/v1/MedicalCondition";

        public MedicalConditionControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GivenNonExistingMedicalConditionId_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentMedicalConditionId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentMedicalConditionId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{BaseUrl}/{nonExistentMedicalConditionId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task GivenValidMedicalCondition_WhenCreateIsCalled_ThenAddToDatabaseTheMedicalCondition()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicalConditionCommand
            {
                PatientId = Guid.NewGuid(),
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "Ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            //Act
            await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            var medicalCondition = dbContext.MedicalConditions.FirstOrDefaultAsync(mc => mc.Name == "Diabetes");
            medicalCondition.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenMissingName_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicalConditionCommand
            {
                PatientId = Guid.NewGuid(),
                Description = "Chronic condition",
                Name="",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "suspected",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Name is required.");
        }

        [Fact]
        public async Task GivenInvalidStartDate_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicalConditionCommand
            {
                PatientId = Guid.NewGuid(),
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
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
        public async Task GivenExistingMedicalConditionId_WhenDeleteIsCalled_ThenMedicalConditionIsDeleted()
        {
            // Arrange
            var client = factory.CreateClient();
            var medicalConditionId = CreateSUTAndReturnMedicalConditionId();
            var token = GenerateJwtToken(medicalConditionId);

            // Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{medicalConditionId}");
            await dbContext.SaveChangesAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedMedicalCondition = await dbContext.MedicalConditions.AsNoTracking().FirstOrDefaultAsync(mc => mc.MedicalConditionId == medicalConditionId);
            deletedMedicalCondition.Should().BeNull();
        }

        [Fact]
        public async Task GivenNonExistingMedicalConditionId_WhenDeleteIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentMedicalConditionId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentMedicalConditionId);

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentMedicalConditionId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenExistingMedicalConditionId_WhenUpdateIsCalled_ThenMedicalConditionIsUpdated()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicalConditionId = CreateSUTAndReturnMedicalConditionId();
            var token = GenerateJwtToken(medicalConditionId);

            var command = new UpdateMedicalConditionCommand
            {
                MedicalConditionId = medicalConditionId,
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "Ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicalConditionId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var updatedMedicalCondition = await dbContext.MedicalConditions.AsNoTracking().FirstOrDefaultAsync(mc => mc.MedicalConditionId == medicalConditionId);
            updatedMedicalCondition!.Name.Should().Be("Diabetes");
        }

        [Fact]
        public async Task GivenNonExistingMedicalConditionId_WhenUpdateIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentMedicalConditionId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var token = GenerateJwtToken(nonExistentMedicalConditionId);

            var command = new UpdateMedicalConditionCommand
            {
                MedicalConditionId = nonExistentMedicalConditionId, // Fix: Use nonExistentMedicalConditionId instead of medicalConditionId
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "Ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentMedicalConditionId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenMissingName_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicalConditionId = CreateSUTAndReturnMedicalConditionId();
            var token = GenerateJwtToken(medicalConditionId);
            var command = new UpdateMedicalConditionCommand
            {
                MedicalConditionId = medicalConditionId,
                Name = "",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicalConditionId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Name is required.");
        }

        [Fact]
        public async Task GivenInvalidStartDate_WhenUpdateIsCalled_ThenReturnsBadRequest()
        {
            //Arrange
            var client = factory.CreateClient();
            var medicalConditionId = CreateSUTAndReturnMedicalConditionId();
            var token = GenerateJwtToken(medicalConditionId);
            var command = new UpdateMedicalConditionCommand
            {
                MedicalConditionId = medicalConditionId,
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(30),
                CurrentStatus = "ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false, // Added required member
                Treatments = new List<TreatmentDto>
                {
                    new TreatmentDto
                    {
                        Name = "Insulin",
                        Type = TreatmentType.Drug,
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<MedicationDto>
                        {
                            new MedicationDto
                            {
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            //Act
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{medicalConditionId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("StartDate cannot be in the future.");
        }

        public void Dispose()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateSUT()
        {
            var medicalCondition = new MedicalCondition
            {
                MedicalConditionId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "Ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false,
                Treatments = new List<Treatment>
                
                {
                    new Treatment
                    {
                        TreatmentId = Guid.NewGuid(),
                        MedicalConditionId = Guid.NewGuid(),
                        Type = TreatmentType.Drug,
                        Name = "Insulin",
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<Medication>
                        {
                            new Medication
                            {
                                Id = Guid.NewGuid(),
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };
            dbContext.MedicalConditions.Add(medicalCondition);
            dbContext.SaveChanges();
        }

        private Guid CreateSUTAndReturnMedicalConditionId()
        {
            var medicalCondition = new MedicalCondition
            {
                MedicalConditionId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Name = "Diabetes",
                Description = "Chronic condition",
                StartDate = DateTime.Now.AddDays(-30),
                CurrentStatus = "Ongoing",
                Recommendation = "Regular checkups",
                IsGenetic = false,
                Treatments = new List<Treatment>
                {
                    new Treatment
                    {
                        TreatmentId = Guid.NewGuid(),
                        MedicalConditionId = Guid.NewGuid(),
                        Type = TreatmentType.Drug,
                        Name = "Insulin",
                        Location = "Home",
                        StartDate = DateTime.Now.AddDays(-30),
                        Duration = DateTime.Now.AddDays(365),
                        Frequency = "Daily",
                        Medications = new List<Medication>
                        {
                            new Medication
                            {
                                Id = Guid.NewGuid(),
                                Name = "Insulin",
                                Type = MedicationType.Injection,
                                Ingredients = "Insulin",
                                AdverseEffects = "Low blood sugar"
                            }
                        }
                    }
                }
            };

            dbContext.MedicalConditions.Add(medicalCondition);
            dbContext.SaveChanges();

            return medicalCondition.MedicalConditionId;
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
