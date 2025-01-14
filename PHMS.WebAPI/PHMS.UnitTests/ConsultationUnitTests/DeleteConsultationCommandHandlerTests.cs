using Application.Use_Cases.CommandHandlers.ConsultationCommandHandler;
using Application.Use_Cases.Commands.ConsultationCommands;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace PHMS.UnitTests.ConsultationUnitTests
{
    public class DeleteConsultationCommandHandlerTests
    {
        private readonly IConsultationRepository repository;
        private readonly DeleteConditionCommandHandler handler;

        public DeleteConsultationCommandHandlerTests()
        {
            repository = Substitute.For<IConsultationRepository>();
            handler = new DeleteConditionCommandHandler(repository);
        }

        [Fact]
        public async Task Given_ValidDeleteConsultationCommand_When_HandlerIsCalled_Then_CommandIsReceived()
        {
            // Arrange
            var idToDelete = Guid.NewGuid();
            var command = new DeleteConsultationCommand(idToDelete);
            var consultation = new Consultation
            {
                Id = idToDelete,
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Location = "Room 101",
                Status = ConsultationStatus.Pending
            };
            repository.GetByIdAsync(idToDelete).Returns(consultation);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).DeleteAsync(idToDelete);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_NonExistentConsultationId_When_HandlerIsCalled_Then_FailureWithErrorMessage()
        {
            // Arrange
            var command = new DeleteConsultationCommand(Guid.NewGuid());
            repository.GetByIdAsync(command.Id).Returns((Consultation?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Consultation not found");
        }
    }
}

