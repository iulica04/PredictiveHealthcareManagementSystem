using Application.Use_Cases.CommandHandlers.ConsultationCommandHandler;
using Application.Use_Cases.Commands.ConsultationCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.ConsultationUnitTests
{
    public class CreateConsultationCommandHandlerTests
    {
        private readonly IConsultationRepository repository;
        private readonly IMapper mapper;
        private readonly CreateConsultationCommandHandler handler;

        public CreateConsultationCommandHandlerTests()
        {
            repository = Substitute.For<IConsultationRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new CreateConsultationCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.AddMinutes(20),
                Location = "Room 101"
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Success(consultation.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).RequestConsultation(consultation);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(consultation.Id);
        }

        [Fact]
        public async Task Given_InvalidDateForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.AddMinutes(10), // Less than 15 minutes in the future
                Location = "Room 101"
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("Consultation date and time must be at least 15 minutes in the future."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).RequestConsultation(consultation);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Consultation date and time must be at least 15 minutes in the future.");
        }

        [Fact]
        public async Task Given_InvalidLocationForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.AddMinutes(20),
                Location = "" // Empty location
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("Location is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).RequestConsultation(consultation);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Location is required.");
        }

        [Fact]
        public async Task Given_LocationExceedingMaxLengthForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.AddMinutes(20),
                Location = new string('a', 101) // Location exceeding 100 characters
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("Location must not exceed 100 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).RequestConsultation(consultation);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Location must not exceed 100 characters.");
        }

        [Fact]
        public async Task Given_InvalidPatientIdForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.Empty, // Invalid PatientId
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.AddMinutes(20),
                Location = "Room 101"
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("PatientId is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.DidNotReceive().RequestConsultation(Arg.Any<Consultation>());
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Given_InvalidMedicIdForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.Empty, // Invalid MedicId
                Date = DateTime.Now.AddMinutes(20),
                Location = "Room 101"
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("MedicId is required."));


            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.DidNotReceive().RequestConsultation(Arg.Any<Consultation>());
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Given_DateOutsideWorkingHoursForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.Date.AddHours(7), // Before 08:00
                Location = "Room 101"
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("Consultation time must be between 08:00 and 18:00."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).RequestConsultation(consultation);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Consultation time must be between 08:00 and 18:00.");
        }

        [Fact]
        public async Task Given_DateOnWeekendForCreateConsultationCommand_When_HandlerIsCalled_Then_ConsultationShouldNotBeCreated()
        {
            // Arrange
            var command = new CreateConsultationCommand
            {
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.Now.AddDays((6 - (int)DateTime.Now.DayOfWeek) % 7 + 1).Date.AddHours(10), // Next Saturday
                Location = "Room 101"
            };
            var consultation = new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                MedicId = command.MedicId,
                Date = command.Date,
                Location = command.Location,
                Status = ConsultationStatus.Pending
            };
            mapper.Map<Consultation>(command).Returns(consultation);
            repository.RequestConsultation(consultation).Returns(Result<Guid>.Failure("Consultation cannot be scheduled on a weekend."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).RequestConsultation(consultation);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Consultation cannot be scheduled on a weekend.");
        }
    }
}
