using Application.Commands.Patient;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using FluentAssertions;
using Application.CommandHandlers.PatientCommandHandlers;
using Domain.Enums;

namespace PHMS.UnitTests.PatientUnitTests
{
    public class CreatePatientCommandHandlerTests
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;
        private readonly CreatePatientCommandHandler handler;

        public CreatePatientCommandHandlerTests()
        {
            repository = Substitute.For<IPatientRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new CreatePatientCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Success(patient.Id));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(patient.Id);
        }


        [Fact]
        public async Task Given_EmptyFistNameForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("First name cannot be empty."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name cannot be empty.");
        }

        [Fact]
        public async Task Given_FirstNameGreaterThan30CharactersForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "ChristopherAlexanderJohnsonWilliams",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("First name must be at most 30 characters."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("First name must be at most 30 characters.");
        }


        [Fact]
        public async Task Given_EmptyLastNameForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Last name cannot be empty."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name cannot be empty.");
        }

        [Fact]
        public async Task Given_LastNameGreaterThan30CharactersForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "McGillicuddySupercalifragilisticexpialidocious",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Last name must be at most 30 characters."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Last name must be at most 30 characters.");
        }


        [Fact]
        public async Task Given_BirthDayGreaterThanTodayForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(2027, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Birth date cannot be greater than today."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Birth date cannot be greater than today.");
        }


        [Fact]
        public async Task Given_InvalidGenderForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "other",
                Email = "ethan.garcia@example.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Gender must be either 'Male' or 'Female'."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Gender must be either 'Male' or 'Female'.");
        }


        [Fact]
        public async Task Given_InvalidEmailForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garciaexample.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Invalid email format."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid email format.");
        }

        [Fact]
        public async Task Given_InvalidPhoneNumberForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "13216aaaa",
                Password = "EthanStrongPass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Invalid phone number format."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Invalid phone number format.");
        }


        [Fact]
        public async Task Given_PasswordLessThan8CharactersForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "+13216549870",
                Password = "Ethan",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Password must be at least 8 characters long."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must be at least 8 characters long.");

        }

        [Fact]
        public async Task Given_PasswordGreaterThan100CharactersForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "+13216549870",
                Password = "A#x1!B2dE%F3Gh*Ij4K5lMn6OpQ7s!T8rU9vWxYzZ0kLmNpR1q@Sv2tWaX3zB@4lMcNvO5xPqRsUtV6dW7yXaZ8b9cJkLmNop0QrStUvWxYz",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Password must be at most 100 characters long."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must be at most 100 characters long.");
        }

        [Fact]
        public async Task Given_PasswordWithoutUppercaseLetterForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "+13216549870",
                Password = "ethanstrongpass_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Password must contain at least one uppercase letter."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);


            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one uppercase letter.");
        }

        [Fact]
        public async Task Given_PasswordWithoutLowercaseLetterForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "+13216549870",
                Password = "ETHANSTR_654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Password must contain at least one lowercase letter."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one lowercase letter.");
        }

        [Fact]
        public async Task Given_PasswordWithoutDigitForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrongPass_",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Password must contain at least one digit."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one digit.");
        }

        [Fact]
        public async Task Given_PasswordWithoutSpecialCharacterForCreatePatientCommand_When_HandleIsCalled_Then_PatientShouldNotBeCreated()
        {
            //Arange
            var command = new CreatePatientCommand
            {
                FirstName = "Ethan",
                LastName = "Garcia",
                BirthDate = new DateTime(1999, 12, 12),
                Gender = "Male",
                Email = "ethan.garcia@exemple.com",
                PhoneNumber = "+13216549870",
                Password = "EthanStrong654",
                Address = "1234 Main St, Springfield, IL 62701"
            };
            var patient = new Patient
            {
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889"),
                Type = UserType.Patient,
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.BirthDate,
                Gender = command.Gender,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Address = command.Address,
                PatientRecords = new List<PatientRecord>()
            };
            mapper.Map<Patient>(command).Returns(patient);
            repository.AddAsync(patient).Returns(Result<Guid>.Failure("Password must contain at least one special character."));

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            await repository.Received(1).AddAsync(patient);
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Password must contain at least one special character.");
        }

    }
}