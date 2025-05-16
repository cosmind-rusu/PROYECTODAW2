using AutoMapper;
using back.Models;
using back.DTOs;

namespace back.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, DTOs.CategoryDto>().ReverseMap();
            CreateMap<Transaction, DTOs.TransactionDto>().ReverseMap();
        }
    }
}
