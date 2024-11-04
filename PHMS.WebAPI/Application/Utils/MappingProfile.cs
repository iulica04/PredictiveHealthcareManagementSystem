using Application.Commands;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<CreatePatientCommand, Patient>().ReverseMap();
            CreateMap<UpdatePatientCommand, Patient>().ReverseMap();
        }
    }
}
