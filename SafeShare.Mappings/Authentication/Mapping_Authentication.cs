/*
 * Defines the AutoMapper mappings for the Authentication domain.
 * This mapping profile converts between DTO_Register and ApplicationUser, and vice-versa.
*/

using AutoMapper;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.Authentication;

namespace SafeShare.Mappings.Authentication;

/// <summary>
/// Defines the AutoMapper mappings for the Authentication domain.
/// </summary>
public class Mapping_Authentication : Profile
{
    public Mapping_Authentication()
    {
        // Mapping from DTO_Register to ApplicationUser
        CreateMap<DTO_Register, ApplicationUser>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.Birthday.Year));

        // Mapping from ApplicationUser to DTO_AuthUser
        CreateMap<ApplicationUser, DTO_AuthUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.Birthday));
    }
}