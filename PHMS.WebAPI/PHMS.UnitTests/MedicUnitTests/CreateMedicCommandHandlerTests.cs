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
    public class CreateMedicCommandHandlerrTests
    {
        private readonly IMedicRepository repository;
        private readonly IMapper mapper;
        private readonly CreateMedicCommandHandler handler;

        public CreateMedicCommandHandlerrTests()
        {
            repository = Substitute.For<IMedicRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new CreateMedicCommandHandler(repository, mapper);
        }

        private static CreateMedicCommand PrepCreateMedicCommand(String custom_field = "none", object? custom_value = null)
        {
#pragma warning disable 8600, 8601, 8605
            Dictionary<String, object?> defaultValues = new()
            {
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

            var command = new CreateMedicCommand
            {
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

        private Medic PrepMedic(CreateMedicCommand command)
        {
            var medic = new Medic
            {
                Id = Guid.NewGuid(),
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
        public async Task Given_ValidCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand();
            Medic medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Success(medic.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(medic.Id);
        }

        [Fact]
        public async Task Given_NoFirstNameForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "FirstName", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("First name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name is required.");
        }

        [Fact]
        public async Task Given_EmptyFirstNameForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "FirstName", custom_value: "");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("First name cannot be empty."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name cannot be empty.");
        }

        [Fact]
        public async Task Given_FirstNameTooLongForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "FirstName", custom_value: new string('a', 31));
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("First name must be at most 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be at most 30 characters.");
        }

        [Fact]
        public async Task Given_NoLastNameForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "LastName", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Last name is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name is required.");
        }

        [Fact]
        public async Task Given_EmptyLastNameForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "LastName", custom_value: "");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Last name cannot be empty."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name cannot be empty.");
        }

        [Fact]
        public async Task Given_LastNameTooLongForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "LastName", custom_value: new string('a', 31));
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Last name must be at most 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name must be at most 30 characters.");
        }

        [Fact]
        public async Task Given_NoEmailForCreateMedicCommand_WhenHandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Email", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Email is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Email is required.");
        }

        [Fact]
        public async Task Given_InvalidEmailForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Email", custom_value: "invalid-email");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Invalid email format."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid email format.");
        }

        [Fact]
        public async Task Given_NoPhoneNumberForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "PhoneNumber", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Phone number is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Phone number is required.");
        }

        [Fact]
        public async Task Given_InvalidPhoneNumberForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "PhoneNumber", custom_value: "invalid-phone");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Invalid phone number format."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid phone number format.");
        }

        [Fact]
        public async Task Given_ShortPasswordForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Password", custom_value: "Short1!");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Password must be at least 8 characters long."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must be at least 8 characters long.");
        }

        [Fact]
        public async Task Given_LongPasswordForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Password", custom_value: new string('a', 101) + "A1!");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Password must be at most 100 characters long."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must be at most 100 characters long.");
        }

        [Fact]
        public async Task Given_PasswordWithoutUppercaseForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Password", custom_value: "password123!");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Password must contain at least one uppercase letter."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter.");
        }

        [Fact]
        public async Task Given_PasswordWithoutLowercaseForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Password", custom_value: "PASSWORD123!");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Password must contain at least one lowercase letter."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one lowercase letter.");
        }

        [Fact]
        public async Task Given_PasswordWithoutDigitForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Password", custom_value: "Password!");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Password must contain at least one digit."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one digit.");
        }

        [Fact]
        public async Task Given_PasswordWithoutSpecialCharacterForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Password", custom_value: "Password123");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Password must contain at least one special character."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one special character.");
        }

        [Fact]
        public async Task Given_NoGenderForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Gender", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Gender is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Gender is required.");
        }

        [Fact]
        public async Task Given_InvalidGenderForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Gender", custom_value: "InvalidGender");
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Gender must be either 'Male' or 'Female'."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Gender must be either 'Male' or 'Female'.");
        }

        [Fact]
        public async Task Given_FutureBirthDateForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "BirthDate", custom_value: DateTime.UtcNow.AddYears(1));
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Birthday must be in the past."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Birthday must be in the past.");
        }

        [Fact]
        public async Task Given_NoAddressForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Address", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Address is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Address is required.");
        }

        [Fact]
        public async Task Given_NoRankForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Rank", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Rank is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Rank is required.");
        }

        [Fact]
        public async Task Given_RankTooLongForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Rank", custom_value: new string('a', 31));
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Rank must be at most 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Rank must be at most 30 characters.");
        }

        [Fact]
        public async Task Given_NoSpecializationForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Specialization", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Specialization is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Specialization is required.");
        }

        [Fact]
        public async Task Given_SpecializationTooLongForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Specialization", custom_value: new string('a', 31));
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Specialization must be at most 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Specialization must be at most 30 characters.");
        }

        [Fact]
        public async Task Given_NoHospitalForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Hospital", custom_value: null);
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Hospital is required."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Hospital is required.");
        }

        [Fact]
        public async Task Given_HospitalTooLongForCreateMedicCommand_When_HandlerIsCalled_Then_MedicShouldNotBeCreated()
        {
            // Arrange
            var command = PrepCreateMedicCommand(custom_field: "Hospital", custom_value: new string('a', 31));
            var medic = PrepMedic(command);
            repository.AddAsync(medic).Returns(Result<Guid>.Failure("Hospital must be at most 30 characters."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            await repository.Received(1).AddAsync(medic);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Hospital must be at most 30 characters.");
        }
    }
}
