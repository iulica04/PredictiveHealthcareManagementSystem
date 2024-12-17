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
using MediatR;

namespace PHMS.UnitTests.UserUnitTests
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly IUserRepository userRepository;
        private readonly DeleteUserCommandHandler handler;

        public DeleteUserCommandHandlerTests()
        {
            userRepository = Substitute.For<IUserRepository>();
            handler = new DeleteUserCommandHandler(userRepository);
        }

        [Fact]
        public async Task Given_ValidDeleteUserCommand_When_HandleIsCalled_Then_AdminShouldBeDeleted()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888");
            var command = new DeleteUserCommand { Id = userId };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(new Admin
            {
                Id = userId,
                Type = UserType.Admin,
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                BirthDate = new DateTime(1985, 5, 15),
                Gender = "Female",
                Email = "old.email@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                PhoneNumber = "0723456789",
                Address = "Old Address"
            })));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).DeleteUserAsync(userId);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_ValidDeleteUserCommand_When_HandleIsCalled_Then_MedicShouldBeDeleted()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new DeleteUserCommand { Id = userId };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(new Medic
            {
                Id = userId,
                Type = UserType.Medic,
                FirstName = "Jane",
                LastName = "Smith",
                BirthDate = new DateTime(1990, 7, 20),
                Gender = "Female",
                Email = "jane.smith@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                PhoneNumber = "0987654321",
                Address = "456 Medic St",
                Specialization = "Cardiology",
                Hospital = "City Hospital",
                Rank = "Senior"
            })));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).DeleteUserAsync(userId);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_ValidDeleteUserCommand_When_HandleIsCalled_Then_PatientShouldBeDeleted()
        {
            // Arrange
            var userId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2890");
            var command = new DeleteUserCommand { Id = userId };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User>.Success(new Patient
            {
                Id = userId,
                Type = UserType.Patient,
                FirstName = "Alice",
                LastName = "Johnson",
                BirthDate = new DateTime(2000, 8, 25),
                Gender = "Female",
                Email = "alice.johnson@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                PhoneNumber = "1122334455",
                Address = "789 Patient St",
                PatientRecords = new List<PatientRecord>()
            })));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await userRepository.Received(1).DeleteUserAsync(userId);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_NonExistentUserId_When_HandleIsCalled_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand { Id = userId };

            userRepository.GetUserByIdAsync(userId).Returns(Task.FromResult(Result<User?>.Failure("User not found")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found");
            result.Data.Should().Be(Unit.Value);
            await userRepository.DidNotReceive().DeleteUserAsync(userId);
        }

        [Fact]
        public async Task Given_InvalidUserId_When_HandleIsCalled_Then_RepositoryShouldNotBeCalled()
        {
            // Arrange
            var invalidId = Guid.Empty;
            var command = new DeleteUserCommand { Id = invalidId };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
            await userRepository.DidNotReceive().DeleteUserAsync(Arg.Any<Guid>());
        }
    }
}



