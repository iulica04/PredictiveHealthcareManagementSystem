﻿using Domain.Common;
using MediatR;

namespace Application.Commands.Patient
{
    public class UpdatePatientCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
    }
}
