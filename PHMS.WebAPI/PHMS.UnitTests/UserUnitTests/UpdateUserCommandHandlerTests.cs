using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Domain.Entities;
using Domain.Enums;
using Application.Use_Cases.Commands.UserCommands;
using Application.Use_Cases.CommandHandlers.UserCommandHandlers;
using Domain.Repositories;
using Domain.Common;
using AutoMapper;

namespace PHMS.UnitTests.UserUnitTests
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly UpdateUserCommandHandler handler;

        public UpdateUserCommandHandlerTests()
        {
            userRepository = Substitute.For<IUserRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new UpdateUserCommandHandler(userRepository, mapper);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAdmin_WhenAdminExists()
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
            var command = new UpdateUserCommand
            {
                Id = userId,
                Type = UserType.Admin,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1985, 6, 15),
                Gender = "Male",
                Email = "john.doe@example.com",
                Password = "newPassword",
                PhoneNumber = "1234567890",
                Address = "123 Admin St"
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            await userRepository.Received(1).UpdateUserAsync(user);
        }

        [Fact]
        public async Task Handle_ShouldUpdateMedic_WhenMedicExists()
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
                Specialization = "Cardiology",
                Hospital = "City Hospital",
                Rank = "Doctor"
            };
            var command = new UpdateUserCommand
            {
                Id = userId,
                Type = UserType.Medic,
                FirstName = "Jane",
                LastName = "Smith",
                BirthDate = new DateTime(1990, 7, 20),
                Gender = "Female",
                Email = "jane.smith@example.com",
                Password = "newPassword",
                PhoneNumber = "0987654321",
                Address = "456 Medic St",
                Specialization = "Cardiology",
                Hospital = "City Hospital"
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            await userRepository.Received(1).UpdateUserAsync(user);
        }

        [Fact]
        public async Task Handle_ShouldUpdatePatient_WhenPatientExists()
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
            var command = new UpdateUserCommand
            {
                Id = userId,
                Type = UserType.Patient,
                FirstName = "Alice",
                LastName = "Johnson",
                BirthDate = new DateTime(2000, 8, 25),
                Gender = "Female",
                Email = "alice.johnson@example.com",
                Password = "newPassword",
                PhoneNumber = "1122334455",
                Address = "789 Patient St"
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            await userRepository.Received(1).UpdateUserAsync(user);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2891");
            var command = new UpdateUserCommand
            {
                Id = userId,
                Type = UserType.Admin,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1985, 6, 15),
                Gender = "Male",
                Email = "john.doe@example.com",
                Password = "newPassword",
                PhoneNumber = "1234567890",
                Address = "123 Admin St"
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User?>.Failure("User not found")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found");
            await userRepository.DidNotReceive().UpdateUserAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldUpdatePassword_WhenPasswordIsProvided()
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
            var command = new UpdateUserCommand
            {
                Id = userId,
                Type = UserType.Admin,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1985, 6, 15),
                Gender = "Male",
                Email = "john.doe@example.com",
                Password = "newPassword",
                PhoneNumber = "1234567890",
                Address = "123 Admin St"
            };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(user)));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            user.PasswordHash.Should().NotBe("hashedPassword");
            await userRepository.Received(1).UpdateUserAsync(user);
        }
    }
}


