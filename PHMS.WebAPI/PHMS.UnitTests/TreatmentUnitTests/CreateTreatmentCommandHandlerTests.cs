using Application.Commands.TreatmentCommands;
using Application.DTOs;
using Application.CommandHandlers.TreatmentCommandHandler;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using FluentValidation.TestHelper;
using NSubstitute;
using Xunit;
using FluentAssertions;
public class CreateTreatmentCommandHandlerTests
{
    private readonly ITreatmentRepository _treatmentRepository;
    private readonly IMedicationRepository _medicationRepository;
    private readonly IMapper _mapper;
    private readonly CreateTreatmentCommandHandler _handler;

    public CreateTreatmentCommandHandlerTests()
    {
        _treatmentRepository = Substitute.For<ITreatmentRepository>();
        _medicationRepository = Substitute.For<IMedicationRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateTreatmentCommandHandler(_treatmentRepository, _medicationRepository, _mapper);
    }

    [Fact]
    public async Task Given_ValidCreateTreatmentCommand_When_HandleIsCalled_Then_TreatmentShouldBeCreated()
    {
        // Arrange
        var command = new CreateTreatmentCommand
        {
            Type = TreatmentType.Drug,
            Name = "Aspirin",
            MedicalConditionId = Guid.NewGuid(),
            Location = "Pharmacy",
            StartDate = DateTime.UtcNow,
            Duration = DateTime.UtcNow.AddDays(10),
            Frequency = "Once a day",
            Medications = new List<MedicationDto>
            {
                new MedicationDto
                {
                    Name = "Aspirin",
                    Type = MedicationType.Tablet,
                    Ingredients = "Acetylsalicylic Acid",
                    AdverseEffects = "Nausea"
                }
            }
        };

        var treatment = new Treatment
        {
            TreatmentId = Guid.NewGuid(),
            Type = command.Type,
            Name = command.Name,
            MedicalConditionId = command.MedicalConditionId,
            Location = command.Location,
            StartDate = command.StartDate,
            Duration = command.Duration,
            Frequency = command.Frequency,
            Medications = new List<Medication>()
        };

        _mapper.Map<Treatment>(command).Returns(treatment);
        _treatmentRepository.AddAsync(treatment).Returns(Result<Guid>.Success(treatment.TreatmentId));

        foreach (var medicationDto in command.Medications)
        {
            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                TreatmentId = treatment.TreatmentId,
                Name = medicationDto.Name,
                Type = medicationDto.Type,
                Ingredients = medicationDto.Ingredients,
                AdverseEffects = medicationDto.AdverseEffects
            };

            _mapper.Map<Medication>(medicationDto).Returns(medication);
            _medicationRepository.AddAsync(medication).Returns(Result<Guid>.Success(medication.Id));
        }

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _treatmentRepository.Received(1).AddAsync(treatment);
        await _medicationRepository.Received(command.Medications.Count).AddAsync(Arg.Any<Medication>());
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(treatment.TreatmentId);
    }

    [Fact]
    public async Task Given_FailedTreatmentCreation_When_HandleIsCalled_Then_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTreatmentCommand
        {
            Type = TreatmentType.Drug,
            Name = "Aspirin",
            MedicalConditionId = Guid.NewGuid(),
            Location = "Pharmacy",
            StartDate = DateTime.UtcNow,
            Duration = DateTime.UtcNow.AddDays(10),
            Frequency = "Once a day",
            Medications = new List<MedicationDto>
            {
                new MedicationDto
                {
                    Name = "Aspirin",
                    Type = MedicationType.Tablet,
                    Ingredients = "Acetylsalicylic Acid",
                    AdverseEffects = "Nausea"
                }
            }
        };

        var treatment = new Treatment
        {
            TreatmentId = Guid.NewGuid(),
            Type = command.Type,
            Name = command.Name,
            MedicalConditionId = command.MedicalConditionId,
            Location = command.Location,
            StartDate = command.StartDate,
            Duration = command.Duration,
            Frequency = command.Frequency,
            Medications = new List<Medication>()
        };

        _mapper.Map<Treatment>(command).Returns(treatment);
        _treatmentRepository.AddAsync(treatment).Returns(Result<Guid>.Failure("Failed to create treatment."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _treatmentRepository.Received(1).AddAsync(treatment);
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to create treatment.");
    }

    [Fact]
    public async Task Given_FailedMedicationCreation_When_HandleIsCalled_Then_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTreatmentCommand
        {
            Type = TreatmentType.Drug,
            Name = "Aspirin",
            MedicalConditionId = Guid.NewGuid(),
            Location = "Pharmacy",
            StartDate = DateTime.UtcNow,
            Duration = DateTime.UtcNow.AddDays(10),
            Frequency = "Once a day",
            Medications = new List<MedicationDto>
            {
                new MedicationDto
                {
                    Name = "Aspirin",
                    Type = MedicationType.Tablet,
                    Ingredients = "Acetylsalicylic Acid",
                    AdverseEffects = "Nausea"
                }
            }
        };

        var treatment = new Treatment
        {
            TreatmentId = Guid.NewGuid(),
            Type = command.Type,
            Name = command.Name,
            MedicalConditionId = command.MedicalConditionId,
            Location = command.Location,
            StartDate = command.StartDate,
            Duration = command.Duration,
            Frequency = command.Frequency,
            Medications = new List<Medication>()
        };

        _mapper.Map<Treatment>(command).Returns(treatment);
        _treatmentRepository.AddAsync(treatment).Returns(Result<Guid>.Success(treatment.TreatmentId));

        var medication = new Medication
        {
            Id = Guid.NewGuid(),
            TreatmentId = treatment.TreatmentId,
            Name = command.Medications[0].Name,
            Type = command.Medications[0].Type,
            Ingredients = command.Medications[0].Ingredients,
            AdverseEffects = command.Medications[0].AdverseEffects
        };

        _mapper.Map<Medication>(command.Medications[0]).Returns(medication);
        _medicationRepository.AddAsync(medication).Returns(Result<Guid>.Failure("Failed to create medication."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _treatmentRepository.Received(1).AddAsync(treatment);
        await _medicationRepository.Received(1).AddAsync(medication);
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Failed to create medication.");
    }
}

