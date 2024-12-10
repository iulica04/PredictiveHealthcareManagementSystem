using Application.DTOs;
using Application.Queries.MedicQueries;
using Application.QueryHandlers.MedicQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.MedicUnitTests
{
    public class GetMedicByIdQueryHandlerTests
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;
        private readonly GetMedicByIdQueryHandler handler;

        public GetMedicByIdQueryHandlerTests()
        {
            repository = Substitute.For<IMedicRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetMedicByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ExistingMedicId_When_HandlerIsCalled_Then_SuccessWithMedicDto()
        {
            // Arrange
            var Medic = GenerateMedic();
            repository.GetByIdAsync(Medic.Id).Returns(Medic);
            var query = new GetMedicByIdQuery { Id = Medic.Id };
            GenerateMedicDto(Medic);

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.Data.Id.Should().Be(Medic.Id);
            result.Data.FirstName.Should().Be(Medic.FirstName);
            result.Data.LastName.Should().Be(Medic.LastName);
            result.Data.BirthDate.Should().Be(Medic.BirthDate);
            result.Data.Gender.Should().Be(Medic.Gender);
            result.Data.Email.Should().Be(Medic.Email);
            result.Data.PhoneNumber.Should().Be(Medic.PhoneNumber);
            result.Data.PasswordHash.Should().Be(Medic.PasswordHash);
            result.Data.Address.Should().Be(Medic.Address);
            result.Data.Rank.Should().Be(Medic.Rank);
            result.Data.Specialization.Should().Be(Medic.Specialization);
            result.Data.Hospital.Should().Be(Medic.Hospital);
        }

        [Fact]
        public async Task Given_NonexistingMedicId_When_HandlerIsCalled_Then_Failure()
        {
            // Arrange
            var MedicId = Guid.NewGuid();
            repository.GetByIdAsync(MedicId).Returns((Medic?)null);
            var query = new GetMedicByIdQuery { Id = MedicId };

            //Act
            var result = await handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Medic with id {MedicId} not found");
        }

        private void GenerateMedicDto(Medic Medic)
        {
            mapper.Map<MedicDto>(Medic).Returns(new MedicDto
            {
                Id = Medic.Id,
                FirstName = Medic.FirstName,
                LastName = Medic.LastName,
                BirthDate = Medic.BirthDate,
                Gender = Medic.Gender,
                Email = Medic.Email,
                PhoneNumber = Medic.PhoneNumber,
                PasswordHash = Medic.PasswordHash,
                Address = Medic.Address,
                Rank = Medic.Rank,
                Specialization = Medic.Specialization,
                Hospital = Medic.Hospital
            });
        }

        private static Medic GenerateMedic()
        {
            return new Medic
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Gender = "Male",
                Address = "123 Main St",
                PasswordHash = "password",
                Rank = "Captain",
                Specialization = "General Practitioner",
                Hospital = "General Hospital"
            };
        }
    }
}