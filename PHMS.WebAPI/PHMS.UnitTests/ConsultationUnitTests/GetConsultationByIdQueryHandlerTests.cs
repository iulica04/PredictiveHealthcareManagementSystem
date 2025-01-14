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
    public class GetConsultationByIdQueryHandlerTests
    {
        private readonly IConsultationRepository repository;
        private readonly IMapper mapper;
        private readonly GetConsultationByIdQueryHandler handler;

        public GetConsultationByIdQueryHandlerTests()
        {
            repository = Substitute.For<IConsultationRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetConsultationByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ExistingConsultationId_When_HandlerIsCalled_Then_SuccessWithConsultationDto()
        {
            // Arrange
            var consultation = GenerateConsultation();
            repository.GetByIdAsync(consultation.Id).Returns(consultation);
            var query = new GetConsultationByIdQuery { Id = consultation.Id };
            GenerateConsultationDto(consultation);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().Be(consultation.Id);
            result.Data.PatientId.Should().Be(consultation.PatientId);
            result.Data.MedicId.Should().Be(consultation.MedicId);
            result.Data.Date.Should().Be(consultation.Date);
            result.Data.Location.Should().Be(consultation.Location);
            result.Data.Status.Should().Be(consultation.Status);
        }

        [Fact]
        public async Task Given_NonExistingConsultationId_When_HandlerIsCalled_Then_FailureWithErrorMessage()
        {
            // Arrange
            var consultationId = Guid.NewGuid();
            repository.GetByIdAsync(consultationId).Returns((Consultation?)null);
            var query = new GetConsultationByIdQuery { Id = consultationId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Consultation with id {consultationId} not found");
        }

        [Fact]
        public async Task Given_InvalidConsultationId_When_HandlerIsCalled_Then_FailureWithErrorMessage()
        {
            // Arrange
            var query = new GetConsultationByIdQuery { Id = Guid.Empty };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Consultation with id 00000000-0000-0000-0000-000000000000 not found");
        }

        private void GenerateConsultationDto(Consultation consultation)
        {
            mapper.Map<ConsultationDto>(consultation).Returns(new ConsultationDto
            {
                Id = consultation.Id,
                PatientId = consultation.PatientId,
                MedicId = consultation.MedicId,
                Date = consultation.Date,
                Location = consultation.Location,
                Status = consultation.Status
            });
        }

        private static Consultation GenerateConsultation()
        {
            return new Consultation
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                MedicId = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(1),
                Location = "Room 101",
                Status = ConsultationStatus.Pending
            };
        }
    }
}
