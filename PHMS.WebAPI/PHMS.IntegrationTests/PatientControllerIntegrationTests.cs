using Application.Commands.Patient;
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
    public class PatientControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/v1/Patient";

        public PatientControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<ApplicationDbContext>));

                    services.Remove(descriptor!);

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
        public async void GivenPatients_WhenGetAllIsCalled_ThenReturnsAllPatients()
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
        public async void GivenExistingPatients_WhenGetAllIsCalled_ThenReturnsAllPatients()
        {
            //Arrange
            var client = factory.CreateClient();
            CreateSUT();

            //Act
            var response = await client.GetAsync(BaseUrl);

            //Assert
            response.EnsureSuccessStatusCode();
            var patients = await response.Content.ReadAsStringAsync();
            patients.Should().Contain("Sophia");

        }

        [Fact]
        public async void GivenExistingPatientId_WhenGetByIdIsCalled_ThenReturnsThePatient()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId(); 

            //Act
            var response = await client.GetAsync($"{BaseUrl}/{patientId}");

            //Assert
            response.EnsureSuccessStatusCode();
            var patients = await response.Content.ReadAsStringAsync();
            patients.Should().Contain("Liam");
        }

        [Fact]
        public async void GivenNonExistingPatientId_WhenGetByIdIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentPatientId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");

            //Act
            var response = await client.GetAsync($"{BaseUrl}/{nonExistentPatientId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GivenValidPatient_WhenCreateIsCalled_ThenAddToDatabaseThePatient()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            var patient = dbContext.Patients.FirstOrDefaultAsync(p => p.FirstName == "Ethan");
            patient.Should().NotBeNull();
        }

        [Fact]
        public async void GivenMissingFirstName_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name cannot be empty.");
        }


        [Fact]
        public async void GivenFirstNameGreaterThan30Characters_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name must be at most 30 characters.");
        }

        [Fact]
        public async void GivenMissingLastName_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name cannot be empty.");
        }

        [Fact]
        public async void GivenLastNameGreaterThan30Characters_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Etahn",
                LastName = "ChristopherAlexanderJohnsonWilliams",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name must be at most 30 characters.");
        }

        [Fact]
        public async void GivenInvalidBirthDate_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Etahn",
                LastName = "Garcia",
                BirthDate = DateTime.Now.AddYears(1),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Birthday must be in the past."); 
        }

        [Fact]
        public async void GivenInvalidGender_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Etahn",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "invalid gender",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Gender must be either 'Male' or 'Female'.");
        }

        [Fact]
        public async void GivenInvalidEmail_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Etahn",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia.example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid email format.");
        }

        [Fact]
        public async void GivenInvalidPhoneNumber_WhenCreateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var command = new CreatePatientCommand
            {
                FirstName = "Etahn",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "abc549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid phone number format.");
        }

        [Fact]
        public async void GivenExistingPatientId_WhenDeleteIsCalled_ThenPatientIsDeleted()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();

            //Act
            var response = await client.DeleteAsync($"{BaseUrl}/{patientId}");
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedPatient = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patientId);
            deletedPatient.Should().BeNull();
        }


        [Fact]
        public async void GivenNonExistingPatientId_WhenDeleteIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentPatientId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");

            //Act
            var response = await client.DeleteAsync($"{BaseUrl}/{nonExistentPatientId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GivenExistingPatientId_WhenUpdateIsCalled_ThenPatientIsUpdated()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Etahn",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };


            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();


            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var updatedPatient = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == patientId);
            updatedPatient!.FirstName.Should().Be("Etahn");
        }

        [Fact]
        public async void GivenNonExistingPatientId_WhenUpdateIsCalled_ThenReturnsNotFound()
        {
            //Arrange
            var client = factory.CreateClient();
            var nonExistentPatientId = new Guid("168da6be-48af-413e-8e25-37aedfcf1f29");
            var command = new UpdatePatientCommand
            {
                Id = nonExistentPatientId,
                FirstName = "Etahn",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{nonExistentPatientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GivenMissingFirstName_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name cannot be empty.");
        }

        [Fact]
        public async void GivenFirstNameGreaterThan30Characters_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("First name must be at most 30 characters.");
        }

        [Fact]
        public async void GivenMissingLastName_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Ethan",
                LastName = "",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name cannot be empty.");
        }

        [Fact]
        public async void GivenLastNameGreaterThan30Characters_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Ethan",
                LastName = "ChristopherAlexanderJohnsonWilliams",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Last name must be at most 30 characters.");
        }

        [Fact]
        public async void GivenInvalidBirthDate_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = DateTime.Now.AddYears(2),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Birthday must be in the past.");
        }

        [Fact]
        public async void GivenInvalidGender_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "invalid gender",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Gender must be either 'Male' or 'Female'.");
        }

        [Fact]
        public async void GivenInvalidEmail_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia.example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid email format.");
        }

        [Fact]
        public async void GivenInvalidPhoneNumber_WhenUpdateIsCalled_ThenReturnsInternalError()
        {
            //Arrange
            var client = factory.CreateClient();
            var patientId = CreateSUTAndReturnPatientId();
            var command = new UpdatePatientCommand
            {
                Id = patientId,
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "aaa16549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            //Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
            await dbContext.SaveChangesAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Invalid phone number format.");
        }

        public void Dispose() 
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateSUT()
        {
            var patient = new Patient
            { 
                FirstName = "Sophia",
                LastName = "Taylor",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                Address = "1234 Main St, Springfield, IL 62701",
                PatientRecords = new List<PatientRecord>()
            };
            dbContext.Patients.Add(patient);
            dbContext.SaveChanges();
        }


        private Guid CreateSUTAndReturnPatientId()
        {
            var patient = new Patient
            {
                FirstName = "Liam",
                LastName = "Miller",
                BirthDate = new DateTime(1995, 5, 20),
                Gender = "Male",
                Email = "liam.miller@example.com",
                PhoneNumber = "+14255533445",
                PasswordHash = "$2a$11$uR1Iu5Pl3auktlQgyWo3LWWa/xNEeFxuCSsAlaVjG3dB8yLh2x2ji", 
                Address = "5678 Oak St, Riverton, NJ 08077",
                PatientRecords = new List<PatientRecord>()
            };

            dbContext.Patients.Add(patient);
            dbContext.SaveChanges();

            return patient.Id;  
        }
    }
}