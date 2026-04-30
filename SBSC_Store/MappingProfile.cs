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
        CreateMap<UserForRegistrationAdminDto, User>();
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product!.Price))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Product!.Price));

        CreateMap<AddToCartDto, CartItem>().ReverseMap();
        CreateMap<UpdateCartItemDto, CartItem>().ReverseMap();
        // Add these lines to your MappingProfile.cs in the CreateMap section:
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.FirstName + " " + src.User!.LastName));
        CreateMap<ReviewForCreationDto, Review>();
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    } 
}