using Application.DTOs;
using Application.Queries.MedialConditionQueries;
using Application.QueryHandlers.MedicalConditionQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;

namespace PHMS.UnitTests.MedicalConditionUnitTests
{
    public class GetMedicalConditionByIdForPatientQueryHandlerTests
    {
        private readonly IMedicalConditionRepository repository;
        private readonly IMapper mapper;
        private readonly GetMedicalConditionByIdForPatientQueryHandler handler;

        public GetMedicalConditionByIdForPatientQueryHandlerTests()
        {
            repository = Substitute.For<IMedicalConditionRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetMedicalConditionByIdForPatientQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ExistingMedicalConditionId_When_HandlerIsCalled_Then_SuccessWithMedicalConditionDto()
        {
            // Arrange
            var medicalCondition = GenerateMedicalCondition();
            var expectedDto = GenerateMedicalConditionDto(medicalCondition);

            repository.GetByIdAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>())
                .Returns(call =>
                {
                    var filter = call.Arg<Expression<Func<MedicalCondition, bool>>>();
                    var compiledFilter = filter.Compile();
                    return compiledFilter(medicalCondition) ? medicalCondition : null;
                });

            mapper.Map<MedicalConditionDto>(medicalCondition).Returns(expectedDto);

            var query = new GetMedicalConditionByIdQuery(medicalCondition.PatientId, medicalCondition.MedicalConditionId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
        }

        [Fact]
        public async Task Given_NonExistentMedicalConditionId_When_HandlerIsCalled_Then_ReturnsNull()
        {
            // Arrange
            var query = new GetMedicalConditionByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            repository.GetByIdAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>()).Returns((MedicalCondition?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
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

        private static MedicalConditionDto GenerateMedicalConditionDto(MedicalCondition medicalCondition)
        {
            return new MedicalConditionDto
            {
                MedicalConditionId = medicalCondition.MedicalConditionId,
                PatientId = medicalCondition.PatientId,
                Name = medicalCondition.Name,
                Description = medicalCondition.Description,
                StartDate = medicalCondition.StartDate,
                EndDate = medicalCondition.EndDate,
                CurrentStatus = medicalCondition.CurrentStatus,
                IsGenetic = medicalCondition.IsGenetic,
                Recommendation = medicalCondition.Recommendation
            };
        }
    }
}
