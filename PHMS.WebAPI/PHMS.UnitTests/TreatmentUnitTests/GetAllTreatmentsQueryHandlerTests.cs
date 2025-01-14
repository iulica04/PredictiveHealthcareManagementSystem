using Application.DTOs;
using Application.Queries.TreatmentQueries;
using Application.QueryHandlers.TreatmentQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.TreatmentUnitTests;
public class GetAllTreatmentsQueryHandlerTests
{
    private readonly ITreatmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly GetAllTreatmentsQueryHandler _handler;

    public GetAllTreatmentsQueryHandlerTests()
    {
        _repository = Substitute.For<ITreatmentRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllTreatmentsQueryHandler(_repository, _mapper);
    }

    [Fact]
    public async Task Given_ValidGetAllTreatmentsQuery_When_HandleIsCalled_Then_ShouldReturnListOfTreatmentDtos()
    {
        // Arrange
        var query = new GetAllTreatmentsQuery();

        var treatments = new List<Treatment>
        {
            new Treatment
            {
                TreatmentId = Guid.NewGuid(),
                Name = "Aspirin",
                Location = "Pharmacy",
                Frequency = "Daily",
                Medications = new List<Medication>()
            },
            new Treatment
            {
                TreatmentId = Guid.NewGuid(),
                Name = "Ibuprofen",
                Location = "Pharmacy",
                Frequency = "Twice a day",
                Medications = new List<Medication>()
            }
        };

        var treatmentDtos = new List<TreatmentDto>
        {
            new TreatmentDto
            {
                Name = "Aspirin",
                Location = "Pharmacy",
                Frequency = "Daily",
                Medications = new List<MedicationDto>()
            },
            new TreatmentDto
            {
                Name = "Ibuprofen",
                Location = "Pharmacy",
                Frequency = "Twice a day",
                Medications = new List<MedicationDto>()
            }
        };

        _repository.GetAllAsync().Returns(treatments);
        _mapper.Map<List<TreatmentDto>>(treatments).Returns(treatmentDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(treatmentDtos);
    }

    [Fact]
    public async Task Given_EmptyTreatmentsList_When_HandleIsCalled_Then_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllTreatmentsQuery();

        var treatments = new List<Treatment>();
        var treatmentDtos = new List<TreatmentDto>();

        _repository.GetAllAsync().Returns(treatments);
        _mapper.Map<List<TreatmentDto>>(treatments).Returns(treatmentDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}