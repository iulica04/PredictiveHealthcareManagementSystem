using Application.CommandHandlers;
using Application.Commands.Administrator;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace PHMS.UnitTests.AdminUnitTests

{
    public class UpdateAdminCommandHandlerTests
    {
        private readonly IAdminRepository repository;
        private readonly IMapper mapper;
        private readonly UpdateAdminCommandHandler handler;

        public UpdateAdminCommandHandlerTests()
        {
            repository = Substitute.For<IAdminRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new UpdateAdminCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidUpdateAdminCommand_When_HandleIsCalled_Then_AdminShouldBeUpdated()
        {
            // Arrange
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateAdminCommand
            {
                Id = adminId,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "updated.email@example.com",
                Password = "newPassword123!",
                PhoneNumber = "0787654321",
                Address = "Updated Address"
            };

            var existingAdmin = new Admin
            {
                Id = adminId,
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                BirthDate = new DateTime(1985, 5, 15),
                Gender = "Female",
                Email = "old.email@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                PhoneNumber = "0723456789",
                Address = "Old Address"
            };

            var updatedAdmin = new Admin
            {
                Id = existingAdmin.Id,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                PhoneNumber = command.PhoneNumber,
                Address = command.Address
            };

            repository.GetByIdAsync(adminId).Returns(existingAdmin);
            mapper.Map(command, existingAdmin).Returns(updatedAdmin);
            repository.UpdateAsync(updatedAdmin).Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).UpdateAsync(updatedAdmin);
            result.IsSuccess.Should().BeTrue();

        }

        [Fact]
        public async Task Given_NonExistentAdminId_When_HandleIsCalled_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var command = new UpdateAdminCommand
            {
                Id = adminId,
                FirstName = "FirstName",
                LastName = "LastName",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                Email = "email@yahoo.com",
                Password = "Password123!",
                PhoneNumber = "0787654321",
                Address = "Address"
            };

            repository.GetByIdAsync(adminId).Returns((Admin)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Admin not found");
            await repository.DidNotReceive().UpdateAsync(Arg.Any<Admin>());
        }


       /* [Fact]
           public async Task Given_UpdateAdminCommandWithInvalidEmail_When_HandleIsCalled_Then_ShouldReturnFailure()
            {
                // Arrange
                var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
                var command = new UpdateAdminCommand
                {
                    Id = adminId,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    BirthDate = new DateTime(1990, 1, 1),
                    Gender = "Male",
                    Email = "emailInvalid.com",  // Invalid email
                    Password = "Password123!",
                    PhoneNumber = "0787654321",
                    Address = "Address"
                };

                var existingAdmin = new Admin
                {
                    Id = adminId,
                    FirstName = "OldFirstName",
                    LastName = "OldLastName",
                    BirthDate = new DateTime(1985, 5, 15),
                    Gender = "Female",
                    Email = "old.email@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                    PhoneNumber = "0723456789",
                    Address = "Old Address"
                };
                var updatedAdmin = new Admin
                {
                    Id = existingAdmin.Id,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    BirthDate = command.BirthDate,
                    Gender = command.Gender,
                    Email = command.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                    PhoneNumber = command.PhoneNumber,
                    Address = command.Address
                };


                repository.GetByIdAsync(adminId).Returns(existingAdmin);
                mapper.Map(command, existingAdmin).Returns(updatedAdmin);
                repository.UpdateAsync(updatedAdmin).Returns(Task.FromResult(Result<Unit>.Failure("Invalid email format.")));

                // Act
                var result = await handler.Handle(command, CancellationToken.None);

                // Assert
                await repository.Received(1).UpdateAsync(updatedAdmin);
                result.IsSuccess.Should().BeFalse();
                result.ErrorMessage.Should().Be("Invalid email format.");

            }*/


           
    } 
 }


