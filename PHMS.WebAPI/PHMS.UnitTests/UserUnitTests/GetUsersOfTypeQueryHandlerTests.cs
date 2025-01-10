using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs;
using Application.Use_Cases.Queries.UserQueries;
using Application.Use_Cases.QueryHandlers.UserQueryHandlers;
using Domain.Repositories;
using Domain.Common;
using AutoMapper;

namespace PHMS.UnitTests.UserUnitTests
{
    public class GetUsersOfTypeQueryHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly GetUsersOfTypeQueryHandler handler;

        public GetUsersOfTypeQueryHandlerTests()
        {
            userRepository = Substitute.For<IUserRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetUsersOfTypeQueryHandler(userRepository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfUserDto_WhenAdminsExist()
        {
            // Arrange
            var users = GenerateAdmins();
            var usersDto = GenerateUserDtos(users);

            userRepository.GetUsersOfTypeAsync(UserType.Admin).Returns(Task.FromResult(Result<IEnumerable<User>>.Success(users)));
            mapper.Map<IEnumerable<UserDto>>(users).Returns(usersDto);

            var query = new GetUsersOfTypeQuery { Type = UserType.Admin };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(usersDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfUserDto_WhenMedicsExist()
        {
            // Arrange
            var users = GenerateMedics();
            var usersDto = GenerateUserDtos(users);

            userRepository.GetUsersOfTypeAsync(UserType.Medic).Returns(Task.FromResult(Result<IEnumerable<User>>.Success(users)));
            mapper.Map<IEnumerable<UserDto>>(users).Returns(usersDto);

            var query = new GetUsersOfTypeQuery { Type = UserType.Medic };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(usersDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfUserDto_WhenPatientsExist()
        {
            // Arrange
            var users = GeneratePatients();
            var usersDto = GenerateUserDtos(users);

            userRepository.GetUsersOfTypeAsync(UserType.Patient).Returns(Task.FromResult(Result<IEnumerable<User>>.Success(users)));
            mapper.Map<IEnumerable<UserDto>>(users).Returns(usersDto);

            var query = new GetUsersOfTypeQuery { Type = UserType.Patient };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(usersDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNoUsersExist()
        {
            // Arrange
            userRepository.GetUsersOfTypeAsync(UserType.Admin).Returns(Task.FromResult(Result<IEnumerable<User>>.Failure("No users found")));

            var query = new GetUsersOfTypeQuery { Type = UserType.Admin };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("No users found");
        }

        private static List<User> GenerateAdmins()
        {
            return new List<User>
            {
                new Admin
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Admin,
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1980, 1, 1),
                    Gender = "Male",
                    Email = "john.doe@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedPassword123"),
                    PhoneNumber = "1234567890",
                    Address = "123 Main St"
                },
                new Admin
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Admin,
                    FirstName = "Jane",
                    LastName = "Smith",
                    BirthDate = new DateTime(1990, 2, 2),
                    Gender = "Female",
                    Email = "jane.smith@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedPassword456"),
                    PhoneNumber = "0987654321",
                    Address = "456 Elm St"
                }
            };
        }

        private static List<User> GenerateMedics()
        {
            return new List<User>
            {
                new Medic
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Medic,
                    FirstName = "Alice",
                    LastName = "Johnson",
                    BirthDate = new DateTime(1985, 3, 3),
                    Gender = "Female",
                    Email = "alice.johnson@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedPassword789"),
                    PhoneNumber = "1122334455",
                    Address = "789 Oak St",
                    Specialization = "Cardiology",
                    Hospital = "City Hospital",
                    Rank = "Senior"
                },
                new Medic
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Medic,
                    FirstName = "Bob",
                    LastName = "Brown",
                    BirthDate = new DateTime(1975, 4, 4),
                    Gender = "Male",
                    Email = "bob.brown@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedPassword012"),
                    PhoneNumber = "6677889900",
                    Address = "101 Pine St",
                    Specialization = "Neurology",
                    Hospital = "County Hospital",
                    Rank = "Junior"
                }
            };
        }

        private static List<User> GeneratePatients()
        {
            return new List<User>
            {
                new Patient
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Patient,
                    FirstName = "Charlie",
                    LastName = "Davis",
                    BirthDate = new DateTime(2000, 5, 5),
                    Gender = "Male",
                    Email = "charlie.davis@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedPassword345"),
                    PhoneNumber = "2233445566",
                    Address = "202 Maple St",
                    PatientRecords = new List<PatientRecord>() // Initialize PatientRecords
                },
                new Patient
                {
                    Id = Guid.NewGuid(),
                    Type = UserType.Patient,
                    FirstName = "Diana",
                    LastName = "Evans",
                    BirthDate = new DateTime(1995, 6, 6),
                    Gender = "Female",
                    Email = "diana.evans@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedPassword678"),
                    PhoneNumber = "3344556677",
                    Address = "303 Birch St",
                    PatientRecords = new List<PatientRecord>() // Initialize PatientRecords
                }
            };
        }

        private static List<UserDto> GenerateUserDtos(List<User> users)
        {
            return users.Select(user => new UserDto
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
                Specialization = user is Medic ? ((Medic)user).Specialization : null,
                Hospital = user is Medic ? ((Medic)user).Hospital : null
            }).ToList();
        }
    }
}

