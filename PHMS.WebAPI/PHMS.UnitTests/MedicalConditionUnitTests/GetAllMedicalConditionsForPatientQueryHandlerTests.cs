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
    public class GetAllMedicalConditionsForPatientQueryHandlerTests
    {
        private readonly IMedicalConditionRepository repository;
        private readonly IMapper mapper;
        private readonly GetAllMedicalConditionsForPatientQueryHandler handler;

        public GetAllMedicalConditionsForPatientQueryHandlerTests()
        {
            repository = Substitute.For<IMedicalConditionRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetAllMedicalConditionsForPatientQueryHandler(repository, mapper);
        }

        private static List<MedicalCondition> GenerateMedicalConditions()
        {
            return new List<MedicalCondition>
            {
                new MedicalCondition
                {
                    MedicalConditionId = Guid.NewGuid(),
                    PatientId = Guid.NewGuid(),
                    Name = "Test Condition 1",
                    Description = "Test Description 1",
                    StartDate = DateTime.Now.AddMonths(-1),
                    EndDate = DateTime.Now,
                    CurrentStatus = "Ongoing",
                    IsGenetic = true,
                    Recommendation = "Test Recommendation 1"
                },
                new MedicalCondition
                {
                    MedicalConditionId = Guid.NewGuid(),
                    PatientId = Guid.NewGuid(),
                    Name = "Test Condition 2",
                    Description = "Test Description 2",
                    StartDate = DateTime.Now.AddMonths(-2),
                    EndDate = DateTime.Now,
                    CurrentStatus = "Cured",
                    IsGenetic = false,
                    Recommendation = "Test Recommendation 2"
                }
            };
        }

        private static List<MedicalConditionDTO> GenerateMedicalConditionDtos(List<MedicalCondition> medicalConditions)
        {
            var dtoList = new List<MedicalConditionDTO>();
            foreach (var mc in medicalConditions)
            {
                dtoList.Add(new MedicalConditionDTO
                {
                    MedicalConditionId = mc.MedicalConditionId,
                    PatientId = mc.PatientId,
                    Name = mc.Name,
                    Description = mc.Description,
                    StartDate = mc.StartDate,
                    EndDate = mc.EndDate,
                    CurrentStatus = mc.CurrentStatus,
                    IsGenetic = (bool)mc.IsGenetic,
                    Recommendation = mc.Recommendation
                });
            }
            return dtoList;
        }

        [Fact]
        public async Task Given_ExistingMedicalConditionsForPatient_When_HandlerIsCalled_Then_ReturnsMedicalConditionDtos()
        {
            // Arrange
            var medicalConditions = GenerateMedicalConditions();
            var expectedDtos = GenerateMedicalConditionDtos(medicalConditions);
            var patientId = medicalConditions[0].PatientId;

            repository.GetAllAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>())
                .Returns(medicalConditions.AsEnumerable()); // Explicit return IEnumerable

            mapper.Map<List<MedicalConditionDTO>>(medicalConditions).Returns(expectedDtos);

            var query = new GetAllMedicalConditionsQuery(patientId);

            // Verificare intermediară pentru `mapper.Map`
            var mappedResult = mapper.Map<List<MedicalConditionDTO>>(medicalConditions);
            mappedResult.Should().NotBeNull();
            mappedResult.Should().BeEquivalentTo(expectedDtos);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDtos);
        }


        [Fact]
        public async Task Given_NoMedicalConditionsForPatient_When_HandlerIsCalled_Then_ReturnsEmptyList()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            repository.GetAllAsync(Arg.Any<Expression<Func<MedicalCondition, bool>>>()).Returns(new List<MedicalCondition>());
            mapper.Map<List<MedicalConditionDTO>>(Arg.Any<List<MedicalCondition>>()).Returns(new List<MedicalConditionDTO>());

            var query = new GetAllMedicalConditionsQuery(patientId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
