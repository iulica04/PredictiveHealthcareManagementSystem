using Application.Commands.MedicalConditionCommands;
using Application.Commands.MedicationCommand;
using Application.Commands.PatientRecordCommands;
using Application.Commands.PrescriptionCommandHandler;
using Application.Commands.TreatmentCommands;
using Application.DTOs;
using Application.Use_Cases.Commands.UserCommands;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Rank, opt => opt.Ignore())
                .ForMember(dest => dest.Specialization, opt => opt.Ignore())
                .ForMember(dest => dest.Hospital, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    switch (src.Type)
                    {
                        case UserType.Medic:
                            var medic = (Medic)src;
                            dest.Rank = medic.Rank;
                            dest.Specialization = medic.Specialization;
                            dest.Hospital = medic.Hospital;
                            break;

                        case UserType.Patient:
                            var patient = (Patient)src;
                            dest.PatientRecords = patient.PatientRecords;
                            break;
                    }
                });
            CreateMap<Patient, UserDto>().IncludeBase<User, UserDto>();
            CreateMap<Medic, UserDto>().IncludeBase<User, UserDto>();
            CreateMap<Admin, UserDto>().IncludeBase<User, UserDto>();


            CreateMap<UpdateUserCommand, User>()
                .Include<UpdateUserCommand, Admin>()
                .Include<UpdateUserCommand, Medic>()
                .Include<UpdateUserCommand, Patient>()
                .AfterMap((src, dest) =>
                {
                    switch (src.Type)
                    {
                        case UserType.Medic:
                            var medic = (Medic)dest;
                            medic.Rank = src.Rank!;
                            medic.Specialization = src.Specialization!;
                            medic.Hospital = src.Hospital!;
                            break;

                        case UserType.Patient:
                            var patient = (Patient)dest;
                            patient.PatientRecords = src.PatientRecords!;
                            break;
                    }
                });
            CreateMap<UpdateUserCommand, Admin>().IncludeBase<UpdateUserCommand, User>();
            CreateMap<UpdateUserCommand, Medic>().IncludeBase<UpdateUserCommand, User>();
            CreateMap<UpdateUserCommand, Patient>().IncludeBase<UpdateUserCommand, User>();

            CreateMap<RegisterCommand, Patient>().ReverseMap();
            CreateMap<RegisterCommand, Medic>().ReverseMap();

            CreateMap<MedicalCondition, MedicalConditionDto>().ReverseMap();
            CreateMap<CreateMedicalConditionCommand, MedicalCondition>().ReverseMap();
            CreateMap<UpdateMedicalConditionCommand, MedicalCondition>().ReverseMap();
            
            CreateMap<Treatment, TreatmentDto>().ReverseMap();
            CreateMap<CreateTreatmentCommand, Treatment>().ReverseMap();
            CreateMap<UpdateTreatmentCommand, Treatment>().ReverseMap();
            
            CreateMap<Medication, MedicationDto>().ReverseMap();
            CreateMap<CreateMedicationCommand, Medication>().ReverseMap();
            CreateMap<UpdateMedicationCommand, Medication>().ReverseMap();
            
            CreateMap<Prescription, PrescriptionDto>().ReverseMap();
            CreateMap<CreatePrescriptionCommand, Prescription>().ReverseMap();
            //CreateMap<UpdatePrescriptionCommand, Prescription>().ReverseMap();

            CreateMap<PatientRecord, PatientRecordDto>().ReverseMap();
            CreateMap<CreatePatientRecordCommand, PatientRecord>().ReverseMap();
            //CreateMap<UpdatePatientRecordCommand, PatientRecord>().ReverseMap();
        }
    }
}
