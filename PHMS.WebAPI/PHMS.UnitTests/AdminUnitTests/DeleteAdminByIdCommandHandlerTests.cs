using Application.Commands.Administrator;
using Application.CommandHandlers;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Domain.Common;

namespace PHMS.UnitTests.AdminUnitTests
{
    public class DeleteAdminByIdCommandHandlerTests
    {
        /*  private readonly IAdminRepository repository;
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
              var adminId = Guid.NewGuid();
              var command = new DeleteAdminByIdCommand(adminId);

              var admin = new Domain.Entities.Admin
              {
                  Id = adminId,

              };

              repository.GetByIdAsync(adminId).Returns(Task.FromResult(admin));

              // Act
              var result = await handler.Handle(command, CancellationToken.None);

              // Assert
              await repository.Received(1).DeleteAsync(adminId);
              result.Should().Be(Result<Unit>.Success(Unit.Value));
          }

          [Fact]
          public async Task Given_NonExistentAdminId_When_HandleIsCalled_Then_FailureResultShouldBeReturned()
          {
              // Arrange
              var adminId = Guid.NewGuid();
              var command = new DeleteAdminByIdCommand(adminId);

              repository.GetByIdAsync(adminId).Returns(Task.FromResult<Domain.Entities.Admin>(null));

              // Act
              var result = await handler.Handle(command, CancellationToken.None);

              // Assert
              result.Should().Be(Result<Unit>.Failure("Admin not found"));
              await repository.DidNotReceive().DeleteAsync(adminId);
          }

          [Fact]
          public async Task Given_InvalidAdminId_When_HandleIsCalled_Then_RepositoryShouldNotBeCalled()
          {
              // Arrange
              var invalidId = Guid.Empty; // or another invalid ID depending on your validation
              var command = new DeleteAdminByIdCommand(invalidId);

              // Act
              Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

              // Assert
              await act.Should().NotThrowAsync();
              await repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
          }
      */
    }

}