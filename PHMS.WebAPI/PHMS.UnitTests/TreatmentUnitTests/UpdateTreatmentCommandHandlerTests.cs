using Application.Commands.TreatmentCommands;
using Application.CommandHandlers.TreatmentCommandHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Application.DTOs;
using Domain.Enums;

namespace PHMS.UnitTests.TreatmentUnitTests;
public class UpdateTreatmentCommandHandlerTests
{
    private readonly ITreatmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly UpdateTreatmentCommandHandler _handler;

    public UpdateTreatmentCommandHandlerTests()
    {
        _repository = Substitute.For<ITreatmentRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateTreatmentCommandHandler(_repository, _mapper);
    }

    [Fact]
    public async Task Given_ValidUpdateTreatmentCommand_When_HandleIsCalled_Then_TreatmentShouldBeUpdated()
    {
        // Arrange
        var command = new UpdateTreatmentCommand
        {
            TreatmentId = Guid.NewGuid(),
            Type = TreatmentType.Drug,
            Name = "Updated Aspirin",
            MedicalConditionId = Guid.NewGuid(),
            Location = "Updated Pharmacy",
            StartDate = DateTime.UtcNow,
            Duration = DateTime.UtcNow.AddDays(10),
            Frequency = "Twice a day",
            Medications = new List<MedicationDto>
            {
                new MedicationDto
                {
                    Name = "Updated Aspirin",
                    Type = MedicationType.Tablet,
                    Ingredients = "Updated Acetylsalicylic Acid",
                    AdverseEffects = "Updated Nausea"
                }
            }
        };

        var treatment = new Treatment
        {
            TreatmentId = command.TreatmentId,
            Type = command.Type,
            Name = command.Name,
            MedicalConditionId = command.MedicalConditionId,
            Location = command.Location,
            StartDate = command.StartDate,
            Duration = command.Duration,
            Frequency = command.Frequency,
            Medications = new List<Medication>()
        };

        _repository.GetByIdAsync(command.TreatmentId).Returns(treatment);
        _mapper.Map(command, treatment).Returns(treatment);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repository.Received(1).UpdateAsync(treatment);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Given_NonExistingTreatmentId_When_HandleIsCalled_Then_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateTreatmentCommand
        {
            TreatmentId = Guid.NewGuid(),
            Type = TreatmentType.Drug,
            Name = "Updated Aspirin",
            MedicalConditionId = Guid.NewGuid(),
            Location = "Updated Pharmacy",
            StartDate = DateTime.UtcNow,
            Duration = DateTime.UtcNow.AddDays(10),
            Frequency = "Twice a day",
            Medications = new List<MedicationDto>
            {
                new MedicationDto
                {
                    Name = "Updated Aspirin",
                    Type = MedicationType.Tablet,
                    Ingredients = "Updated Acetylsalicylic Acid",
                    AdverseEffects = "Updated Nausea"
                }
            }
        };

        _repository.GetByIdAsync(command.TreatmentId).Returns((Treatment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Treatment not found");
    }
}


