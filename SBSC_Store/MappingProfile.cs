using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace SBSC_Store;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<Product, ProductDto>();
        CreateMap<CategoryForCreationDto, Category>();
        CreateMap<ProductForCreationDto, Product>();
        CreateMap<ProductForUpdateDto, Product>().ReverseMap();
        CreateMap<CategoryForUpdateDto, Category>();
        CreateMap<UserForRegistrationDto, User>();
    } 
}