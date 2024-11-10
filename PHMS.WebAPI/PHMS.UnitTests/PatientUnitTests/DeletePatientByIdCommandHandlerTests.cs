using Application.CommandHandlers;
using Application.Commands.Patient;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace PHMS.UnitTests.PatientUnitTests
{
    public class DeletePatientByIdCommandHandlerTests
    {
        private readonly IPatientRepository repository;
        private readonly DeletePatientByIdCommandHandler handler;

        public DeletePatientByIdCommandHandlerTests()
        {
            repository = Substitute.For<IPatientRepository>();
            handler = new DeletePatientByIdCommandHandler(repository);
        }

        [Fact]
        public async Task Given_ValidDeletePatientByIdCommand_When_HandleIsCalled_Then_PatientShouldBeDeleted()
        {
            // Arrange
            var patientId = new Guid("9c922454-33a3-498f-ad9d-d62173cd3bef");
            var command = new DeletePatientByIdCommand(patientId);

            repository.GetByIdAsync(patientId).Returns(new Patient { Id = patientId });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).DeleteAsync(patientId);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_InvalidDeletePatientByIdCommand_When_HandleIsCalled_Then_PatientShouldNotBeDeleted()
        {
            // Arrange
            var patientId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888");
            var command = new DeletePatientByIdCommand(patientId);

            repository.GetByIdAsync(patientId).Returns((Patient)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.DidNotReceive().DeleteAsync(patientId);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Patient not found");
        }
    }
}
