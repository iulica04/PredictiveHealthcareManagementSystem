using Application.Commands.MedicalConditionCommands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using System.Linq.Expressions;

namespace PHMS.UnitTests.MedicalConditionUnitTests
{
    public class UpdateMedicalConditionCommandHandlerTests
    {
        private readonly IMedicalConditionRepository repository;
        private readonly IMapper mapper;
        private readonly Application.CommandHandlers.MedicalConditionCommandHandlers.UpdateMedicalConditionCommandHandler handler;

        public UpdateMedicalConditionCommandHandlerTests()
        {
            repository = Substitute.For<IMedicalConditionRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new Application.CommandHandlers.MedicalConditionCommandHandlers.UpdateMedicalConditionCommandHandler(repository, mapper);
        }

        private static MedicalCondition GenerateMedicalCondition()
        {
            return new MedicalCondition
            {
                MedicalConditionId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Name = "Existing Medical Condition",
                Description = "Existing Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                CurrentStatus = "Existing Status",
                IsGenetic = true,
                Recommendation = "Existing Recommendation"
            };
        }

        [Fact]
        public async Task Given_ExistingMedicalConditionId_When_HandlerIsCalled_Then_SuccessWithUnit()
        {
            // Arrange
            var medicalCondition = GenerateMedicalCondition();
            var command = new UpdateMedicalConditionCommand
            {
                MedicalConditionId = medicalCondition.MedicalConditionId,
                PatientId = medicalCondition.PatientId,
                Name = "Updated Medical Condition",
                Description = "Updated Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                CurrentStatus = "Updated Status",
                IsGenetic = false,
                Recommendation = "Updated Recommendation"
            };

            repository.GetByIdAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>())
                .Returns(call =>
                {
                    var filter = call.Arg<Expression<Func<MedicalCondition, bool>>>();
                    var compiledFilter = filter.Compile();
                    return compiledFilter(medicalCondition) ? medicalCondition : null;
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);

            // Verify that UpdateAsync was called with the modified entity
            await repository.Received(1).UpdateAsync(medicalCondition);

            // Verify that the entity was updated with the command data
            mapper.Received(1).Map(command, medicalCondition);
        }

        [Fact]
        public async Task Given_NonExistentMedicalConditionId_When_HandlerIsCalled_Then_FailureWithErrorMessage()
        {
            // Arrange
            var command = new UpdateMedicalConditionCommand
            {
                MedicalConditionId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Name = "Non-existent Medical Condition",
                Description = "Non-existent Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                CurrentStatus = "Non-existent Status",
                IsGenetic = false,
                Recommendation = "Non-existent Recommendation"
            };

            repository.GetByIdAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>()).Returns((MedicalCondition)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Medical condition not found");

            // Verify that UpdateAsync was not called
            await repository.DidNotReceive().UpdateAsync(Arg.Any<MedicalCondition>());
        }
    }
}
