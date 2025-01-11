using Application.CommandHandlers.PatientCommandHandlers;
using Application.Commands.Patient;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.PatientUnitTests
{
    public class UpdatePatientCommandHandlerTests
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;
        private readonly UpdatePatientCommandHandler handler;

        public UpdatePatientCommandHandlerTests()
        {
            this.repository = Substitute.For<IPatientRepository>();
            this.mapper = Substitute.For<IMapper>();
            this.handler = new UpdatePatientCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidUpdatePatientCommand_WhenPatientExists_Then_ShouldUpdatePatient()
        {
            // Arrange
            var command = new UpdatePatientCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Sophia",
                LastName = "Taylor",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            var existingPatient = new Patient
            {
                Id = command.Id,
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Female",
                Email = "old.email@example.com",
                PhoneNumber = "+1234567890",
                PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                Address = "Old Address",
                PatientRecords = new List<PatientRecord>()
            };

            repository.GetByIdAsync(command.Id).Returns(existingPatient);
            var updatedPatient = new Patient
            {
                Id = command.Id,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map(command, existingPatient).Returns(updatedPatient);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).UpdateAsync(Arg.Is<Patient>(p =>
                p.Id == command.Id &&
                p.FirstName == command.FirstName &&
                p.LastName == command.LastName &&
                p.BirthDate == command.BirthDate &&
                p.Gender == command.Gender &&
                p.Email == command.Email &&
                p.PhoneNumber == command.PhoneNumber &&
                p.PasswordHash == updatedPatient.PasswordHash &&
                p.Address == command.Address
            ));
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Given_ValidUpdatePatientCommand_WhenPatientDoesNotExists_Then_ShouldUpdatePatient()
        {
            // Arrange
            var command = new UpdatePatientCommand
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                FirstName = "Sophia",
                LastName = "Taylor",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };

            repository.GetByIdAsync(command.Id).Returns((Patient?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.DidNotReceive().UpdateAsync(Arg.Any<Patient>());
            result.IsSuccess.Should().BeFalse();
        }
    }
}

