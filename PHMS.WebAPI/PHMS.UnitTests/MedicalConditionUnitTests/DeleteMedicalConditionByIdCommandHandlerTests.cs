using Application.CommandHandlers.MedicalConditionCommandHandlers;
using Application.Commands.MedicalConditionCommands;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using System.Linq.Expressions;

namespace PHMS.UnitTests.MedicalConditionUnitTests
{
    public class DeleteMedicalConditionByIdCommandHandlerTests
    {
        private readonly IMedicalConditionRepository repository;
        private readonly DeleteMedicalConditionByIdCommandHandler handler;

        public DeleteMedicalConditionByIdCommandHandlerTests()
        {
            repository = Substitute.For<IMedicalConditionRepository>();
            handler = new DeleteMedicalConditionByIdCommandHandler(repository);
        }

        private static MedicalCondition GenerateMedicalCondition()
        {
            return new MedicalCondition
            {
                MedicalConditionId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                Name = "Test Medical Condition",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                CurrentStatus = "Test Status",
                IsGenetic = true,
                Recommendation = "Test Recommendation",
                Treatments = new List<Treatment>()
            };
        }

        [Fact]
        public async Task Given_ExistingMedicalConditionId_When_HandlerIsCalled_Then_SuccessWithUnit()
        {
            // Arrange
            var medicalCondition = GenerateMedicalCondition();
            var command = new DeleteMedicalConditionByIdCommand { Id = medicalCondition.MedicalConditionId };

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

            await repository.Received(1).DeleteAsync(command.Id);
        }

        [Fact]
        public async Task Given_NonExistentMedicalConditionId_When_HandlerIsCalled_Then_FailureWithErrorMessage()
        {
            // Arrange
            var command = new DeleteMedicalConditionByIdCommand { Id = Guid.NewGuid() };

            repository.GetByIdAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>()).Returns((MedicalCondition?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Medical condition not found");

            // Verify that DeleteAsync was not called
            await repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }
    }
}
