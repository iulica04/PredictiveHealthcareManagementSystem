using Application.CommandHandlers.MedicalConditionCommandHandlers;
using Application.Commands.MedicalConditionCommands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;

namespace PHMS.UnitTests.MedicalConditionUnitTests
{
    public class CreateMedicalConditionCommandHandlerTests
    {
        private readonly IMedicalConditionRepository repository;
        private readonly CreateMedicalConditionCommandHandler handler;
        private readonly IMapper mapper;

        public CreateMedicalConditionCommandHandlerTests()
        {
            repository = Substitute.For<IMedicalConditionRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new CreateMedicalConditionCommandHandler(repository, mapper);
        }

        private static CreateMedicalConditionCommand PrepCreateMedicalCondition()
        {
            return new CreateMedicalConditionCommand
            {
                PatientId = Guid.NewGuid(),
                Name = "Test Medical Condition",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = null,
                CurrentStatus = "ongoing",
                IsGenetic = false,
                Recommendation = "Test Recommendation"

            };
        }

        [Fact]
        public async Task Given_ValidCreateMedicalConditionCommand_When_HandlerIsCalled_Then_CommandIsReceived()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation,
                //Treatments = new List<Treatment>() // Initialize the required Treatments property
            };
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Success(Guid.NewGuid()));

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(Arg.Any<MedicalCondition>());
        }

        [Fact]
        public async Task Given_EmptyGuidForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("Invalid Id format"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Id format", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_EmptyNameForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.Name = string.Empty;
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("Name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Name is required.", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_EmptyDescriptionForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.Description = string.Empty;
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("Description is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Description is required.", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_EmptyStartDateForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.StartDate = DateTime.Now;
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("StartDate cannot be in the future."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("StartDate cannot be in the future.", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_EndDateBeforeStartDateForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.EndDate = DateTime.Now.AddDays(-1);
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("EndDate must be after StartDate."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("EndDate must be after StartDate.", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_InvalidCurrentStatusForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.CurrentStatus = "invalid";
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("CurrentStatus must be one of the following: 'ongoing', 'cured', 'suspected', 'inactive'."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("CurrentStatus must be one of the following: 'ongoing', 'cured', 'suspected', 'inactive'.", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_NullIsGeneticForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.IsGenetic = true;
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("IsGenetic must be specified."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("IsGenetic must be specified.", result.ErrorMessage);
        }

        [Fact]
        public async Task Given_EmptyRecommendationForCreateMedicalConditionCommand_When_HandlerIsCalled_Then_FailureIsReturned()
        {
            // Arrange
            var command = PrepCreateMedicalCondition();
            command.Recommendation = string.Empty;
            var medicalCondition = new MedicalCondition
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Description = command.Description,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CurrentStatus = command.CurrentStatus,
                IsGenetic = command.IsGenetic,
                Recommendation = command.Recommendation
            };
            mapper.Map<MedicalCondition>(command).Returns(medicalCondition);
            repository.AddAsync(Arg.Any<MedicalCondition>()).Returns(Result<Guid>.Failure("Recommendations are required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Recommendations are required.", result.ErrorMessage);
        }


    }
}
