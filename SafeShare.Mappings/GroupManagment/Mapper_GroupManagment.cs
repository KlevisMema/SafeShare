using AutoMapper;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.GroupManagment;
using SafeShare.DataTransormObject.GroupManagment.GroupInvitations;

namespace SafeShare.Mappings.GroupManagment;

public class Mapper_GroupManagment : Profile
{
    public Mapper_GroupManagment()
    {
        CreateMap<DTO_CreateGroup, Group>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GroupDescription))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));


        CreateMap<GroupMember, DTO_GroupType>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

        CreateMap<List<GroupMember>, DTO_GroupsTypes>()
            .ForMember(dest => dest.GroupsJoined, opt => opt.MapFrom(src => MapToDTOGroupType(src, false)))
            .ForMember(dest => dest.GroupsCreated, opt => opt.MapFrom(src => MapToDTOGroupType(src, true)));


        CreateMap<GroupInvitation, DTO_RecivedInvitations>()
            .ForMember(dest => dest.InvitationMessage, opt => opt.MapFrom(src => src.InvitationMessage))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.InvitingUserId, opt => opt.MapFrom(src => Guid.Parse(src.InvitingUserId)))
            .ForMember(dest => dest.InvitingUserName, opt => opt.MapFrom(src => src.InvitingUser.FullName));


        CreateMap<GroupInvitation, DTO_SentInvitations>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.InvitationTimeSend, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.InvitationStatus, opt => opt.MapFrom(src => src.InvitationStatus))
            .ForMember(dest => dest.InvitedUserId, opt => opt.MapFrom(src => src.InvitedUserId))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.InvitedUser.FullName))
            .ForMember(dest => dest.InvitationId, opt => opt.MapFrom(src => src.Id));
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