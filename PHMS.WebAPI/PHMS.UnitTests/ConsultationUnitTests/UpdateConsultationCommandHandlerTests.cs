using Application.Use_Cases.CommandHandlers.ConsultationCommandHandler;
using Application.Use_Cases.Commands.ConsultationCommands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.ConsultationUnitTests
{
    public class UpdateConsultationCommandHandlerTests
    {
        private readonly IConsultationRepository repository;
        private readonly IMapper mapper;
        private readonly UpdateConsultationCommandHandler handler;

        public UpdateConsultationCommandHandlerTests()
        {
            repository = Substitute.For<IConsultationRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new UpdateConsultationCommandHandler(repository, mapper);
        }

        private static UpdateConsultationCommand PrepUpdateConsultationCommand(string customField = "none", object? customValue = null)
        {
#pragma warning disable 8600, 8601, 8605
            Dictionary<string, object?> defaultValues = new()
            {
                { "Id", Guid.NewGuid() },
                { "PatientId", Guid.NewGuid() },
                { "MedicId", Guid.NewGuid() },
                { "Date", DateTime.UtcNow.AddMinutes(20) },
                { "Location", "Room 101" },
                { "Status", ConsultationStatus.Pending }
            };
            if (customField != "none")
            {
                defaultValues[customField] = customValue;
            }

            var command = new UpdateConsultationCommand
            {
                Id = (Guid)defaultValues["Id"],
                PatientId = (Guid)defaultValues["PatientId"],
                MedicId = (Guid)defaultValues["MedicId"],
                Date = (DateTime)defaultValues["Date"],
                Location = (string)defaultValues["Location"],
                Status = (ConsultationStatus)defaultValues["Status"]
            };

            return command;
#pragma warning restore 8600, 8601, 8605
        }

        private Consultation PrepConsultation(UpdateConsultationCommand command)
        {
            var consultation = new Consultation
            {
                Id = command.Id,
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = command.Status
            };

            mapper.Map<Consultation>(command).Returns(consultation);

            return consultation;
        }

        [Fact]
        public async Task Given_ValidUpdateConsultationCommand_When_HandlerIsCalled_Then_ConsultationIsUpdated()
        {
            // Arrange
            var command = PrepUpdateConsultationCommand();
            var consultation = PrepConsultation(command);
            mapper.Map(command, Arg.Any<Consultation>()).Returns(consultation);
            repository.GetByIdAsync(command.Id).Returns(consultation);
            repository.UpdateAsync(consultation).Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await repository.Received(1).UpdateAsync(consultation);
        }

        [Fact]
        public async Task Given_NonExistentConsultationId_When_HandlerIsCalled_Then_ConsultationIsNotUpdated()
        {
            // Arrange
            var command = PrepUpdateConsultationCommand();
            repository.GetByIdAsync(command.Id).Returns((Consultation?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Consultation not found.");
            await repository.DidNotReceive().UpdateAsync(Arg.Any<Consultation>());
        }
    }
}
