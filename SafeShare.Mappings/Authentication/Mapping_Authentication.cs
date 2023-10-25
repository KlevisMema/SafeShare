using AutoMapper;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Mappings.Authentication;

public class Mapping_Authentication : Profile
{
    public Mapping_Authentication() 
    {
        CreateMap<DTO_Register, ApplicationUser>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.Birthday.Year));
            
    }
}