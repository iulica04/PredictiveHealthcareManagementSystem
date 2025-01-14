using Application.DTOs;
using Application.Queries.TreatmentQueries;
using Application.QueryHandlers.TreatmentQueryHandlers;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class GetTreatmentByIdQueryHandlerTests
{
    private readonly ITreatmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly GetTreatmentByIdQueryHandler _handler;

    public GetTreatmentByIdQueryHandlerTests()
    {
        _repository = Substitute.For<ITreatmentRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetTreatmentByIdQueryHandler(_repository, _mapper);
    }

    [Fact]
    public async Task Given_ValidGetTreatmentByIdQuery_When_HandleIsCalled_Then_ShouldReturnTreatmentDto()
    {
        // Arrange
        var query = new GetTreatmentByIdQuery
        {
            TreatmentId = Guid.NewGuid()
        };

        var treatment = new Treatment
        {
            TreatmentId = query.TreatmentId,
            Name = "Aspirin",
            Location = "Pharmacy",
            Frequency = "Daily",
            Medications = new List<Medication>()
        };

        var treatmentDto = new TreatmentDto
        {
            Name = "Aspirin",
            Location = "Pharmacy",
            Frequency = "Daily",
            Medications = new List<MedicationDto>()
        };

        _repository.GetByIdAsync(query.TreatmentId).Returns(treatment);
        _mapper.Map<TreatmentDto>(treatment).Returns(treatmentDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(treatmentDto);
    }

    [Fact]
    public async Task Given_NonExistingTreatmentId_When_HandleIsCalled_Then_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetTreatmentByIdQuery
        {
            TreatmentId = Guid.NewGuid()
        };

        _repository.GetByIdAsync(query.TreatmentId).Returns((Treatment)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Treatment not found");
    }
}



