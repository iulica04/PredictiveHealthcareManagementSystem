using Application.DTOs;
using Application.Queries;
using Application.QueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.AdminUnitTests
{
    public class GetAdminByIdQueryHandlerTests
    {
        private readonly IAdminRepository adminRepository;
        private readonly IMapper mapper;

        public GetAdminByIdQueryHandlerTests()
        {
            adminRepository = Substitute.For<IAdminRepository>();
            mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public async Task Handle_ShouldReturnAdminDto_WhenAdminExists()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var query = new GetAdminByIdQuery { Id = adminId };
            var handler = new GetAdminByIdQueryHandler(adminRepository, mapper);

            var admin = GenerateAdmin(adminId);
            adminRepository.GetByIdAsync(adminId).Returns(admin);

            var adminDto = GenerateAdminDto(admin);
            mapper.Map<AdminDto>(admin).Returns(adminDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(adminDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAdminDoesNotExist()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var query = new GetAdminByIdQuery { Id = adminId };
            var handler = new GetAdminByIdQueryHandler(adminRepository, mapper);

            adminRepository.GetByIdAsync(adminId).Returns((Admin)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Admin not found");
        }

        private Admin GenerateAdmin(Guid adminId)
        {
            return new Admin
            {
                Id = adminId,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1985, 6, 15),
                Gender = "Male",
                Email = "john.doe@example.com",
                PasswordHash = "hashedPassword",
                PhoneNumber = "1234567890",
                Address = "123 Admin St"
            };
        }

        private AdminDto GenerateAdminDto(Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                BirthDate = admin.BirthDate,
                Gender = admin.Gender,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Address = admin.Address
            };
        }
    }
}