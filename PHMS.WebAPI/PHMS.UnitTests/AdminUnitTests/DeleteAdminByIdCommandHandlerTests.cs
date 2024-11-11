using Application.Commands.Administrator;
using Application.CommandHandlers;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Domain.Common;
using Domain.Entities;

namespace PHMS.UnitTests.AdminUnitTests

{
    public class DeleteAdminByIdCommandHandlerTests
    {
          private readonly IAdminRepository repository;
          private readonly DeleteAdminByIdCommandHandler handler;

          public DeleteAdminByIdCommandHandlerTests()
          {
              repository = Substitute.For<IAdminRepository>();
              handler = new DeleteAdminByIdCommandHandler(repository);
          }

          [Fact]
          public async Task Given_ValidDeleteAdminByIdCommand_When_HandleIsCalled_Then_AdminShouldBeDeleted()
          {
              // Arrange
              
              var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888");
              var command = new DeleteAdminByIdCommand(adminId);

              repository.GetByIdAsync(adminId).Returns(new Admin { 
                  Id = adminId,
                  FirstName = "OldFirstName",
                  LastName = "OldLastName",
                  BirthDate = new DateTime(1985, 5, 15),
                  Gender = "Female",
                  Email = "old.email@example.com",
                  PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPassword123!"),
                  PhoneNumber = "0723456789",
                  Address = "Old Address"
              });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

              // Assert
              await repository.Received(1).DeleteAsync(adminId);
              result.IsSuccess.Should().BeTrue();
              result.Data.Should().Be(Unit.Value);
        }

        [Fact]
        public async Task Given_NonExistentAdminId_When_HandleIsCalled_Then_FailureResultShouldBeReturned()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var command = new DeleteAdminByIdCommand(adminId);

            repository.GetByIdAsync(adminId).Returns(Task.FromResult<Admin>(null));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Admin not found");
            result.Data.Should().Be(Unit.Value); 
            await repository.DidNotReceive().DeleteAsync(adminId);
        }

        [Fact]
          public async Task Given_InvalidAdminId_When_HandleIsCalled_Then_RepositoryShouldNotBeCalled()
          {
              // Arrange
              var invalidId = Guid.Empty; 
              var command = new DeleteAdminByIdCommand(invalidId);

            // Act
              Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

              // Assert
              await act.Should().NotThrowAsync();
              await repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
          }
      
    }

}