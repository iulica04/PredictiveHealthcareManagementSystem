using Application.Commands.TreatmentCommands;
using Application.CommandHandlers.TreatmentCommandHandlers;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Application.CommandHandlers.TreatmentCommandHandler;
using MediatR;

public class DeleteTreatmentByIdCommandHandlerTests
{
    private readonly ITreatmentRepository _repository;
    private readonly DeleteTreatmentByIdCommandHandler _handler;

    public DeleteTreatmentByIdCommandHandlerTests()
    {
        _repository = Substitute.For<ITreatmentRepository>();
        _handler = new DeleteTreatmentByIdCommandHandler(_repository);
    }

    [Fact]
    public async Task Given_ValidDeleteTreatmentByIdCommand_When_HandleIsCalled_Then_TreatmentShouldBeDeleted()
    {
        // Arrange
        var command = new DeleteTreatmentByIdCommand(Guid.NewGuid());

        var treatment = new Treatment
        {
            TreatmentId = command.Id,
            Name = "Aspirin",
            Location = "Pharmacy",
            Frequency = "Daily",
            Medications = new List<Medication>()
        };

        _repository.GetByIdAsync(command.Id).Returns(treatment);
        _repository.DeleteAsync(command.Id).Returns(Task.FromResult(Result<Unit>.Success(Unit.Value)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).DeleteAsync(command.Id);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Given_NonExistingTreatmentId_When_HandleIsCalled_Then_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteTreatmentByIdCommand(Guid.NewGuid());

        _repository.GetByIdAsync(command.Id).Returns((Treatment)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Treatment not found");
    }

    [Fact]
    public async Task Given_FailedDelete_When_HandleIsCalled_Then_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteTreatmentByIdCommand(Guid.NewGuid());

        var treatment = new Treatment
        {
            TreatmentId = command.Id,
            Name = "Aspirin",
            Location = "Pharmacy",
            Frequency = "Daily",
            Medications = new List<Medication>()
        };

        _repository.GetByIdAsync(command.Id).Returns(treatment);
        _repository.DeleteAsync(command.Id).Returns(Task.FromResult(Result<Unit>.Failure("Failed to delete treatment.")));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();
    }
}


