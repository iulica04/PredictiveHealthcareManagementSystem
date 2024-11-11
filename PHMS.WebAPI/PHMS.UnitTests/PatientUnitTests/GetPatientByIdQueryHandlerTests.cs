
﻿using Application.DTOs;
using Application.Queries;
using Application.QueryHandlers;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace PHMS.UnitTests.PatientUnitTests

{
    public class GetPatientByIdQueryHandlerTests
    {
        private readonly IPatientRepository repository;
        private readonly IMapper mapper;
        public GetPatientByIdQueryHandlerTests()
        {
            this.repository = Substitute.For<IPatientRepository>();
            this.mapper = Substitute.For<IMapper>();
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
