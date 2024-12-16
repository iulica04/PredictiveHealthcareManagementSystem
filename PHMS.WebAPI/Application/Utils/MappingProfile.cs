using Application.Commands.MedicalConditionCommands;
using Application.Commands.MedicationCommand;
using Application.Commands.PatientRecordCommands;
using Application.Commands.PrescriptionCommandHandler;
using Application.Commands.TreatmentCommands;
using Application.DTOs;
using Application.Use_Cases.Authentification;
using AutoMapper;
using Domain.Entities;

namespace Application.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Patient, UserDto>().ReverseMap();
            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<RegisterUserCommand, User>().ReverseMap();

            CreateMap<Medic, UserDto>().ReverseMap();
            CreateMap<Medic, MedicDto>().ReverseMap();
            CreateMap<RegisterMedicCommand, Medic>().ReverseMap();

            CreateMap<Admin, UserDto>().ReverseMap();
            CreateMap<Admin, AdminDto>().ReverseMap();

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
