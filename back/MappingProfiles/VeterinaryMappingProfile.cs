using AutoMapper;
using back.DTOs;
using back.Models;

namespace back.MappingProfiles
{
    public class VeterinaryMappingProfile : Profile
    {
        public VeterinaryMappingProfile()
        {
            CreateMap<AnimalSpecies, AnimalSpeciesDto>().ReverseMap();
            CreateMap<VeterinaryConsultation, VeterinaryConsultationDto>().ReverseMap();
            CreateMap<Treatment, TreatmentDto>().ReverseMap();
            CreateMap<HealthPlan, HealthPlanDto>().ReverseMap();
        }
    }
}
