using AutoMapper;
using PatientsAPI.Models;
using PatientsAPI.Models.Dto;

namespace PatientsAPI.AutoMapperProfiles;

public class PatientProfiles : Profile
{
    public PatientProfiles()
    {
        CreateMap<PatientDto, Patient>()
            .ForPath(d => d.Id, opt => opt.MapFrom(s => s.Name.Id))
            .ForPath(d => d.Use, opt => opt.MapFrom(s => s.Name.Use))
            .ForPath(d => d.Family, opt => opt.MapFrom(s => s.Name.Family))
            .ForPath(d => d.Given, opt => opt.MapFrom(s => string.Join(',', s.Name.Given)));

        CreateMap<Patient, PatientDto>()
           .ForPath(d => d.Name.Id, opt => opt.MapFrom(s => s.Id))
           .ForPath(d => d.Name.Use, opt => opt.MapFrom(s => s.Use))
           .ForPath(d => d.Name.Family, opt => opt.MapFrom(s => s.Family))
           .ForPath(d => d.Name.Given, opt => opt.MapFrom(s => s.Given.Split(',', StringSplitOptions.RemoveEmptyEntries)));
    }
}
