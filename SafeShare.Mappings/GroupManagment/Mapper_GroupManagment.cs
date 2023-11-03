using AutoMapper;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.Mappings.GroupManagment;

public class Mapper_GroupManagment : Profile
{
    public Mapper_GroupManagment()
    {
        CreateMap<DTO_CreateGroup, Group>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GroupDescription))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));


        CreateMap<GroupMember, DTO_GroupType>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

        CreateMap<List<GroupMember>, DTO_GroupsTypes>()
            .ForMember(dest => dest.GroupsJoined, opt => opt.MapFrom(src => MapToDTOGroupType(src, false)))
            .ForMember(dest => dest.GroupsCreated, opt => opt.MapFrom(src => MapToDTOGroupType(src, true)));
    }

    private List<DTO_GroupType>
    MapToDTOGroupType
    (
        List<GroupMember> groupMembers,
        bool isOwner
    )
    {
        return groupMembers
            .Where(x => x.IsOwner == isOwner)
            .Select(x => new DTO_GroupType
            {
                GroupId = x.GroupId,
                GroupName = x.Group.Name
            })
            .ToList();
    }
}