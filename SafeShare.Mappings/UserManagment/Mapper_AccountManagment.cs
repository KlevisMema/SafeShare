using AutoMapper;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.Mappings.UserManagment;

public class Mapper_AccountManagment : Profile
{
    public Mapper_AccountManagment()
    {
        CreateMap<ApplicationUser, DTO_UserInfo>();

        CreateMap<DTO_UserInfo, ApplicationUser>()
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.Birthday.Year));

        CreateMap<ApplicationUser, DTO_UserUpdatedInfo>()
           .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}