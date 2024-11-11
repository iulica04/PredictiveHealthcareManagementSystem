﻿using Application.DTOs;
using Application.Queries;
using Application.QueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHMS.UnitTests.PatientUnitTests

{
    public class GetPatientsQueryHandlerTests
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;
        public GetPatientsQueryHandlerTests()
        {
            this.repository = Substitute.For<IPatientRepository>();
            this.mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public void Given_GetPatientsQueryHandler_When_HandleIsCalled_Then_AListOfPatientsShouldBeReturned()
        {
            // Arrange
            List<Patient> patients = GeneratePatiens();
            repository.GetAllAsync().Returns(patients);
            var query = new GetAllPatientsQuery();
            GeneratePatientsDto(patients);

            //Act
            var handler = new GetAllPatientsQueryHandler(repository, mapper);
            var result = handler.Handle(query, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            Assert.Equal(2, result.Result.Count);
            Assert.Equal(patients[0].Id, result.Result[0].Id);
            Assert.Equal(patients[1].Id, result.Result[1].Id);

        }
        private void GeneratePatientsDto(List<Patient> patients)
        {
            mapper.Map<List<PatientDto>>(patients).Returns(new List<PatientDto>
            {
                new PatientDto
                {
                    Id = patients[0].Id,
                    FirstName = patients[0].FirstName,
                    LastName = patients[0].LastName,
                    BirthDate = patients[0].BirthDate,
                    Gender = patients[0].Gender,
                    Email = patients[0].Email,
                    PhoneNumber = patients[0].PhoneNumber,
                    PasswordHash = patients[0].PasswordHash,
                    Address = patients[0].Address,
                    PatientRecords = patients[0].PatientRecords
                },
                new PatientDto
                {
                    Id = patients[1].Id,
                    FirstName = patients[1].FirstName,
                    LastName = patients[1].LastName,
                    BirthDate = patients[1].BirthDate,
                    Gender = patients[1].Gender,
                    Email = patients[1].Email,
                    PhoneNumber = patients[1].PhoneNumber,
                    PasswordHash = patients[1].PasswordHash,
                    Address = patients[1].Address,
                    PatientRecords = patients[1].PatientRecords
                }
            });
        }

        private List<Patient> GeneratePatiens()
        {
            return new List<Patient>
            {
                new Patient
                {
                    Id = Guid.Parse("9c922454-33a3-498f-ad9d-d62173cd3bef"),
                    FirstName = "Sophia",
                    LastName = "Taylor",
                    BirthDate = DateTime.Parse("1982-05-21T10:11:56.985Z"),
                    Gender = "Female",
                    Email = "sophia.taylor@example.com",
                    PhoneNumber = "+14445556667",
                    PasswordHash = "$2a$11$Vp3mxEdei672TlcjmWTdPel.OHNrHyd746E2nytTgg7rx7Q7pXb0C",
                    Address = "505 Birch Boulevard, Anywhere, USA",
                    PatientRecords = new List<PatientRecord>()
                },
                new Patient
                {
                    Id = Guid.Parse("3abc6383-9e12-4ca3-8005-f0674a7c28a4"),
                    FirstName = "Ethan",
                    LastName = "Wilson",
                    BirthDate = DateTime.Parse("1978-11-19T10:11:56.985Z"),
                    Gender = "Male",
                    Email = "ethan.wilson@example.com",
                    PhoneNumber = "+15557778889",
                    PasswordHash = "$2a$11$7UCJnuDKaRjNUhudVoX7XOEVFZKKPLglD74JHzCWKveoIfrJBaHei",
                    Address = "606 Maple Drive, Everytown, USA",
                    PatientRecords = new List<PatientRecord>()
                }
            };
        }
    }
}

