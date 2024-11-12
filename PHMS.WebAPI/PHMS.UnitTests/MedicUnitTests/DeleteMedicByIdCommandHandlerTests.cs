//using Application.CommandHandlers;
//using Application.Commands.Medic;
//using Domain.Entities;
//using Domain.Repositories;
//using NSubstitute;

//namespace PHMS.UnitTests.MedicUnitTests
//{
//    public class DeleteMedicByIdCommandHandlerTests
//    {
//        private readonly IMedicRepository repository;
//        private readonly DeleteMedicByIdCommandHandler handler;

//        public DeleteMedicByIdCommandHandlerTests()
//        {
//            repository = Substitute.For<IMedicRepository>();
//            handler = new DeleteMedicByIdCommandHandler(repository);
//        }

        [Fact]
        public async Task Given_ValidDeleteMedicByIdCommand_When_HandlerIsCalled_Then_CommandIsReceived()
        {
           // Arrange
           var idToDelete = Guid.NewGuid();
           // var command = new DeleteMedicByIdCommand { Id = idToDelete };
            var medic = new Medic
            {
                Id = idToDelete,
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
            repository.GetByIdAsync(idToDelete).Returns(medic);

           //  Act
           await handler.Handle(command, CancellationToken.None);

//           // Assert
//           await repository.Received(1).DeleteAsync(idToDelete);
//        }

        [Fact]
        public async Task Given_NoIdForDeleteMedicByIdCommand_When_HandlerIsCalled_Then_CommandIsNotReceived()
        {
           // Arrange
           var command = new DeleteMedicByIdCommand();

           // Act
           await handler.Handle(command, CancellationToken.None);

//            // Assert
//            await repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
//        }
//    }
//}