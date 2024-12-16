using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using NSubstitute;

namespace PHMS.UnitTests.PatientUnitTests
{
    public class GetPatientByIdQueryHandlerTests
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;
        public GetPatientByIdQueryHandlerTests()
        {
            this.repository = Substitute.For<IPatientRepository>();
            this.mapper = Substitute.For<IMapper>();
        }

        /*[Fact]
        public async Task Given_GetPatientByIdQueryHandler_When_HandleIsCalled_Then_APatientShouldBeReturned()
        {
            // Arrange
            var patient = GeneratePatien();
            repository.GetByIdAsync(patient.Id).Returns(patient);
            var query = new GetPatientByIdQuery{ Id = patient.Id};
            GeneratePatientDto(patient);
            var handler = new GetPatientByIdQueryHandler(repository, mapper);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Data.Id.Should().Be(patient.Id);
            result.Data.FirstName.Should().Be(patient.FirstName);
            result.Data.LastName.Should().Be(patient.LastName);
            result.Data.BirthDate.Should().Be(patient.BirthDate);
            result.Data.Gender.Should().Be(patient.Gender);
            result.Data.Email.Should().Be(patient.Email);
            result.Data.PhoneNumber.Should().Be(patient.PhoneNumber);
            result.Data.PasswordHash.Should().Be(patient.PasswordHash);
            result.Data.Address.Should().Be(patient.Address);
            result.Data.PatientRecords.Should().BeEquivalentTo(patient.PatientRecords);
        }*/

        /*[Fact]
        public async Task Given_GetPatientByIdQueryHandler_When_HandleIsCalledWithInvalidId_Then_ShouldThrowException()
        {
            // Arrange
            var patientId = new Guid("9c922454-33a3-498f-ad9d-d62173cd3bef");
            repository.GetByIdAsync(patientId).Returns((Patient?)null);
            var query = new GetPatientByIdQuery { Id = patientId };
            var handler = new GetPatientByIdQueryHandler(repository, mapper);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
        }*/

        private void GeneratePatientDto(Patient patient)
        {
            mapper.Map<PatientDto>(patient).Returns(new PatientDto
            {
                Id = patient.Id,
                Type = patient.Type,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                PasswordHash = patient.PasswordHash,
                Address = patient.Address,
                PatientRecords = patient.PatientRecords
            });
        }

        private static Patient GeneratePatien()
        {
            return new Patient
            {
                Id = new Guid("9c922454-33a3-498f-ad9d-d62173cd3bef"),
                Type = UserType.Patient,
                FirstName = "Sophia",
                LastName = "Taylor",
                BirthDate = DateTime.Parse("1982-05-21T10:11:56.985Z"),
                Gender = "Female",
                Email = "sophia.taylor@example.com",
                PhoneNumber = "+14445556667",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                Address = "505 Birch Boulevard, Anywhere, USA",
                PatientRecords = new List<PatientRecord>()
            };
        }


    }
}
