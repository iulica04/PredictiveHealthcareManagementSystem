using Application.CommandHandlers;
using Application.Commands.Medic;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using FluentAssertions;

namespace PHMS.UnitTests.MedicUnitTests
{
    public class UpdateMedicCommandHandlerrTests
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;
        private readonly UpdateMedicCommandHandler handler;

        public UpdateMedicCommandHandlerrTests()
        {
            repository = Substitute.For<IMedicRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new UpdateMedicCommandHandler(repository, mapper);
        }

        private static UpdateMedicCommand PrepUpdateMedicCommand(String custom_field = "none", object? custom_value = null)
        {
#pragma warning disable 8600, 8601, 8605
            Dictionary<String, object?> defaultValues = new()
            {
                { "Id", Guid.NewGuid() },
                { "FirstName", "John" },
                { "LastName", "Doe" },
                { "BirthDate", DateTime.UtcNow.AddYears(-30) },
                { "Gender", "Male" },
                { "Email", "john.doe@example.com" },
                { "PhoneNumber", "1234567890" },
                { "Password", "Password123!" },
                { "Address", "123 Main St" },
                { "Rank", "Captain" },
                { "Specialization", "Cardiology" },
                { "Hospital", "General Hospital" }
            };
            if (custom_field != "none")
            {
                defaultValues[custom_field] = custom_value;
            }

            var command = new UpdateMedicCommand
            {
                Id = (Guid)defaultValues["Id"],
                FirstName = (string)defaultValues["FirstName"],
                LastName = (string)defaultValues["LastName"],
                BirthDate = (DateTime)defaultValues["BirthDate"],
                Gender = (string)defaultValues["Gender"],
                Email = (string)defaultValues["Email"],
                PhoneNumber = (string)defaultValues["PhoneNumber"],
                Password = (string)defaultValues["Password"],
                Address = (string)defaultValues["Address"],
                Rank = (string)defaultValues["Rank"],
                Specialization = (string)defaultValues["Specialization"],
                Hospital = (string)defaultValues["Hospital"]
            };

            return command;
#pragma warning restore 8600, 8601, 8605
        }

        private Medic PrepMedic(UpdateMedicCommand command)
        {
            var medic = new Medic
            {
                Id = command.Id,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                Rank = command.Rank,
                Specialization = command.Specialization,
                Hospital = command.Hospital
            };

            mapper.Map<Medic>(command).Returns(medic);

            return medic;
        }

        [Fact]
        public async Task Given_ValidUpdateMedicCommand_When_HandlerIsCalled_Then_MedicsIsUpdated()
        {
            // Arrange
            var command = PrepUpdateMedicCommand();
            var medic = PrepMedic(command);
            repository.GetByIdAsync(command.Id).Returns(medic);
            repository.UpdateAsync(medic).Returns(Task.CompletedTask);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            medic.Id.Should().Be(command.Id);
            await repository.Received(1).UpdateAsync(medic);
        }

        [Fact]
        public async Task Given_InvalidUpdateMedicCommand_When_HandlerIsCalled_Then_MedicIsNotUpdated()
        {
            // Arrange
            var command = PrepUpdateMedicCommand(custom_field: "Hospital", custom_value: null);
            var medic = PrepMedic(command);
            repository.GetByIdAsync(command.Id).Returns((Medic?)null);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.DidNotReceive().UpdateAsync(medic);
        }
    }
}
