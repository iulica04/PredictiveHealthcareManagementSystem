
﻿using Application.DTOs;
using Application.Queries;
using Application.QueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.AdminUnitTests

{
        public class GetAllAdminsQueryHandlerTests
        {
            private readonly IAdminRepository adminRepository;
            private readonly IMapper mapper;

            public GetAllAdminsQueryHandlerTests()
            {
                adminRepository = Substitute.For<IAdminRepository>();
                mapper = Substitute.For<IMapper>();
             
            }

            [Fact]
            public void Given_GetAllAdminsQueryHandler_When_HandleIsCalled_Then_AListOfAdminsDtoShouldBeReturned()
            {
                // Arrange
                List<Admin> admins = GenerateAdmins();
                adminRepository.GetAllAsync().Returns(admins);

                var query = new GetAllAdminsQuery();
                GenerateAdminsDto(admins);

               // Act
                var handler = new GetAllAdminsQueryHandler(adminRepository, mapper);
                var result =  handler.Handle(query, CancellationToken.None);

                // Assert
                result.Should().NotBeNull();
                Assert.Equal(2, result.Result.Count);
                Assert.Equal(admins[0].Id, result.Result[0].Id);

        }

            private void GenerateAdminsDto(List<Admin> admins)
            {
                mapper.Map<List<AdminDto>>(admins).Returns(new List<AdminDto>
            {
                new AdminDto
                {
                    Id = admins[0].Id,
                    FirstName = admins[0].FirstName,
                    LastName = admins[0].LastName,
                    BirthDate = admins[0].BirthDate,
                    Gender = admins[0].Gender,
                    Email = admins[0].Email,
                    PhoneNumber = admins[0].PhoneNumber,
                    Address = admins[0].Address
                },
                new AdminDto
                {
                    Id = admins[1].Id,
                    FirstName = admins[1].FirstName,
                    LastName = admins[1].LastName,
                    BirthDate = admins[1].BirthDate,
                    Gender = admins[1].Gender,
                    Email = admins[1].Email,
                    PhoneNumber = admins[1].PhoneNumber,
                    Address = admins[1].Address
                }
            });
            }

            private List<Admin> GenerateAdmins()
            {
                return new List<Admin>
            {
                new Admin
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1980, 1, 1),
                    Gender = "Male",
                    Email = "john.doe@example.com",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St"
                },
                new Admin
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Jane",
                    LastName = "Smith",
                    BirthDate = new DateTime(1990, 2, 2),
                    Gender = "Female",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "0987654321",
                    Address = "456 Elm St"
                }
            };
            }
        }
   }

