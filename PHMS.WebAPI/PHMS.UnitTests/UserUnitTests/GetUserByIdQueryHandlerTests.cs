using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs;
using Application.Use_Cases.Queries.UserQueries;
using Domain.Repositories;
using Domain.Common;
using AutoMapper;
using Application.Use_Cases.QueryHandlers.UserQueryHandlers;

namespace PHMS.UnitTests.UserUnitTests
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly GetUserByIdQueryHandler handler;

        public GetUserByIdQueryHandlerTests()
        {
            userRepository = Substitute.For<IUserRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetUserByIdQueryHandler(userRepository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserDto_WhenAdminExists()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888");
            var user = new Admin
            {
                Id = userId,
                Type = UserType.Admin,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1985, 6, 15),
                Gender = "Male",
                Email = "john.doe@example.com",
                PasswordHash = "hashedPassword",
                PhoneNumber = "1234567890",
                Address = "123 Admin St"
            };
            var userDto = new UserDto
            {
                Id = user.Id,
                Type = user.Type,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));
            mapper.Map<UserDto>(user).Returns(userDto);

            var query = new GetUserByIdQuery { Id = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserDto_WhenMedicExists()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var user = new Medic
            {
                Id = userId,
                Type = UserType.Medic,
                FirstName = "Jane",
                LastName = "Smith",
                BirthDate = new DateTime(1990, 7, 20),
                Gender = "Female",
                Email = "jane.smith@example.com",
                PasswordHash = "hashedPassword",
                PhoneNumber = "0987654321",
                Address = "456 Medic St",
                Rank = "Senior",
                Specialization = "Cardiology",
                Hospital = "City Hospital"
            };
            var userDto = new UserDto
            {
                Id = user.Id,
                Type = user.Type,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Rank = user.Rank,
                Specialization = user.Specialization,
                Hospital = user.Hospital
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));
            mapper.Map<UserDto>(user).Returns(userDto);

            var query = new GetUserByIdQuery { Id = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserDto_WhenPatientExists()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2890");
            var user = new Patient
            {
                Id = userId,
                Type = UserType.Patient,
                FirstName = "Alice",
                LastName = "Johnson",
                BirthDate = new DateTime(2000, 8, 25),
                Gender = "Female",
                Email = "alice.johnson@example.com",
                PasswordHash = "hashedPassword",
                PhoneNumber = "1122334455",
                Address = "789 Patient St",
                PatientRecords = new List<PatientRecord>()
            };
            var userDto = new UserDto
            {
                Id = user.Id,
                Type = user.Type,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                PatientRecords = user.PatientRecords
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));
            mapper.Map<UserDto>(user).Returns(userDto);

            var query = new GetUserByIdQuery { Id = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var query = new GetUserByIdQuery { Id = userId };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User?>.Failure("User not found")));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found");
        }
    }
}


