using Application.Commands.MedicationCommand;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace PHMS.IntegrationTests
{
    public class MedicationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;
        private readonly string BaseUrl = "api/v1/Medication";

        public MedicationControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            var scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }


        [Fact]
        public async Task GivenMissingName_WhenCreateIsCalled_ThenReturnsBadRequest()
        {
            // Arrange
            var client = factory.CreateClient();
            var command = new CreateMedicationCommand
            {
                TreatmentId = Guid.NewGuid(),
                Name = string.Empty,
                Type = MedicationType.Tablet,
                Ingredients = "Acetylsalicylic Acid",
                AdverseEffects = "Nausea"
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task GivenNonExistingMedicationId_WhenDeleteIsCalled_ThenReturnsNotFound()
        {
            // Arrange
            var client = factory.CreateClient();
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



        [Fact]
        public async Task GivenNonExistingMedicationId_WhenUpdateIsCalled_ThenReturnsNotFound()
        {
            // Arrange
            var client = factory.CreateClient();
            var nonExistingId = Guid.NewGuid();
            var command = new UpdateMedicationCommand
            {
                Id = nonExistingId,
                Name = "Updated Name",
                Type = MedicationType.Capsule,
                Ingredients = "Updated Ingredients",
                AdverseEffects = "Updated Adverse Effects"
            };

            // Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistingId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private Guid CreateSUTAndReturnMedicationId()
        {
            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                TreatmentId = Guid.NewGuid(),
                Name = "Test Medication",
                Type = MedicationType.Tablet,
                Ingredients = "Test Ingredients",
                AdverseEffects = "Test Adverse Effects"
            };
            dbContext.Medications.Add(medication);
            dbContext.SaveChanges();
            return medication.Id;
        }
    }
}