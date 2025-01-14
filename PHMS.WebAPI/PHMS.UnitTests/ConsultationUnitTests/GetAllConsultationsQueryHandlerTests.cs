using Application.DTOs;
using Application.Use_Cases.Queries.ConsultationsQueries;
using Application.Use_Cases.QueryHandlers.ConsultationsQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.ConsultationUnitTests
{
    public class GetAllConsultationsQueryHandlerTests
    {
        private readonly IConsultationRepository repository;
        private readonly IMapper mapper;
        private readonly GetAllConsultationsQueryHandler handler;

        public GetAllConsultationsQueryHandlerTests()
        {
            repository = Substitute.For<IConsultationRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetAllConsultationsQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task When_HandlerIsCalled_Then_ReturnsListOfConsultationDtos()
        {
            // Arrange
            var consultations = GenerateConsultations();
            repository.GetAllAsync().Returns(consultations);
            var query = new GetAllConsultationsQuery();
            GenerateConsultationDtos(consultations);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(consultations.Count);
            result[0].Id.Should().Be(consultations[0].Id);
            result[1].Id.Should().Be(consultations[1].Id);
        }

        [Fact]
        public async Task When_NoConsultationsExist_Then_ReturnsEmptyList()
        {
            // Arrange
            var consultations = new List<Consultation>();
            repository.GetAllAsync().Returns(consultations);
            var query = new GetAllConsultationsQuery();
            GenerateConsultationDtos(consultations);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        private void GenerateConsultationDtos(List<Consultation> consultations)
        {
            if (consultations == null || consultations.Count == 0)
                mapper.Map<List<ConsultationDto>>(consultations).Returns(new List<ConsultationDto>());

            mapper.Map<List<ConsultationDto>>(consultations).Returns(consultations!.Select(consultation => new ConsultationDto
            {
                Id = consultation.Id,
                PatientId = consultation.PatientId,
                MedicId = consultation.MedicId,
                Date = consultation.Date,
                Location = consultation.Location,
                Status = consultation.Status
            }).ToList());
        }

        private static List<Consultation> GenerateConsultations()
        {
            return new List<Consultation>()
            {
                new Consultation
                {
                    Id = Guid.NewGuid(),
                    PatientId = Guid.NewGuid(),
                    MedicId = Guid.NewGuid(),
                    Date = DateTime.UtcNow.AddDays(1),
                    Location = "Room 101",
                    Status = ConsultationStatus.Pending
                },
                new Consultation
                {
                    Id = Guid.NewGuid(),
                    PatientId = Guid.NewGuid(),
                    MedicId = Guid.NewGuid(),
                    Date = DateTime.UtcNow.AddDays(2),
                    Location = "Room 102",
                    Status = ConsultationStatus.Accepted
                }
            };
        }
    }
}
