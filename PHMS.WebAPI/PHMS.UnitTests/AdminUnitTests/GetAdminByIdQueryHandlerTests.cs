using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
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

        /*[Fact]
        public async Task Handle_ShouldReturnAdminDto_WhenAdminExists()
        {
            // Arrange
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888");
            var admins = GenerateAdmins();
            adminRepository.GetAllAsync().Returns(admins);
            var query = new GetAdminByIdQuery { Id = adminId };
            var handler = new GetAdminByIdQueryHandler(adminRepository, mapper);

            var admin = admins.Where(a => a.Id == adminId).ToList();
            var adminDto = GenerateAdminDto(admin);
            mapper.Map<List<AdminDto>>(admin).Returns(adminDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
  
        }*/

        /*[Fact]
        public async Task Handle_ShouldReturnFailure_WhenAdminDoesNotExist()
        {
            // Arrange
            var adminId = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2889");
            var query = new GetAdminByIdQuery { Id = adminId };
            var handler = new GetAdminByIdQueryHandler(adminRepository, mapper);

            adminRepository.GetByIdAsync(adminId).Returns((Admin?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Admin not found");
        }*/

        private static List<Admin> GenerateAdmins()
        {
            return new List<Admin>
            {
              new(){
                Id = new Guid("0550c1dc-df3f-4dc2-9e29-4388582d2888"),
                Type = UserType.Admin,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1985, 6, 15),
                Gender = "Male",
                Email = "john.doe@example.com",
                PasswordHash = "hashedPassword",
                PhoneNumber = "1234567890",
                Address = "123 Admin St"
              }
            };
        }

        private static List<AdminDto> GenerateAdminDto(List<Admin> admins)
        {
            return admins.Select(admin => new AdminDto
            {
                Id = admin.Id,
                Type = admin.Type,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                BirthDate = admin.BirthDate,
                Gender = admin.Gender,
                Email = admin.Email,
                PasswordHash = admin.PasswordHash,
                PhoneNumber = admin.PhoneNumber,
                Address = admin.Address
            }).ToList();
        }
    }
}