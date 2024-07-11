// WebApi/MappingProfile.cs
using Application.DTOs;
using AutoMapper;
using Core.Entities;

namespace WebApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
