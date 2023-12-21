using AutoMapper;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.Mappings.SafeShareApi.ExpenseManagment;

public class Mapper_ExpenseManagment : Profile
{
    public Mapper_ExpenseManagment()
    {
        CreateMap<DTO_ExpenseCreate, Expense>()
            .ForMember(dest => dest.Desc, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId));

        CreateMap<DTO_ExpenseCreate, DTO_Expense>();

        CreateMap<Expense, DTO_Expense>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Desc));
    }
}