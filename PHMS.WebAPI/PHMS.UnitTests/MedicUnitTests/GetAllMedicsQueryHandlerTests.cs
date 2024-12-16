using Application.DTOs;
using Application.Queries.MedicQueries;
using Application.QueryHandlers.MedicQueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.MedicUnitTests
{
    public class GetAllMedicsQueryHandlerTests
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;
        private readonly GetAllMedicsQueryHandler handler;

        public GetAllMedicsQueryHandlerTests()
        {
            repository = Substitute.For<IMedicRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetAllMedicsQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task When_HandlerIsCalled_Then_AListOfMedicsIsReturned()
        {
            // Arrange
            var medics = GenerateMedics();
            GenerateMedicDtos(medics);
            repository.GetAllAsync().Returns(medics);

            // Act
            var result = await handler.Handle(new GetAllMedicsQuery(), CancellationToken.None);

            // Assert
            await repository.Received(1).GetAllAsync();
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Id.Should().Be(medics[0].Id);
            result[1].Id.Should().Be(medics[1].Id);
        }

        private static List<Medic> GenerateMedics()
        {
            return new List<Medic>()
            {
                new Medic
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Medic,
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
                },
                new Medic
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Medic,
                    FirstName = "Jane",
                    LastName = "Doe",
                    BirthDate = DateTime.UtcNow.AddYears(-25),
                    Email = "jane.doe@example.com",
                    PhoneNumber = "9876543210",
                    Gender = "Female",
                    Address = "22 Second Ave",
                    PasswordHash = "strongerPassword12!",
                    Rank = "Assistant",
                    Specialization = "Neurosurgery",
                    Hospital = "General Hospital"
                }
            };
        }

        private void GenerateMedicDtos(List<Medic> medics)
        {
            if (medics == null || medics.Count == 0)
                mapper.Map<List<MedicDto>>(medics).Returns([]);

            mapper.Map<List<MedicDto>>(medics).Returns(medics!.Select(medic => new MedicDto
            {
                Id = medic.Id,
                Type = medic.Type,
                FirstName = medic.FirstName,
                LastName = medic.LastName,
                BirthDate = medic.BirthDate,
                Gender = medic.Gender,
                Email = medic.Email,
                PhoneNumber = medic.PhoneNumber,
                PasswordHash = medic.PasswordHash,
                Address = medic.Address,
                Rank = medic.Rank,
                Specialization = medic.Specialization,
                Hospital = medic.Hospital
            }).ToList());
        }
    }
}
