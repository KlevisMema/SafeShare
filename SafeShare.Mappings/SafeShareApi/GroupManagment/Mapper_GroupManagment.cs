/* 
 * Provides mapping configurations for the Group Management module. This class defines AutoMapper 
 * profiles to convert between Data Transfer Objects (DTOs) and database models related to group 
 * management functionalities.
 */

using AutoMapper;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment;
using SafeShare.DataTransormObject.SafeShareApi.GroupManagment.GroupInvitations;

namespace SafeShare.Mappings.SafeShareApi.GroupManagment;

/// <summary>
/// Configures AutoMapper mappings for the Group Management module.
/// Defines the mappings between Data Transfer Objects (DTOs) and entity models for group management operations.
/// </summary>
public class Mapper_GroupManagment : Profile
{
    /// <summary>
    /// Initializes a new instance of the Mapper_GroupManagment class, setting up the mapping configurations.
    /// </summary>
    public Mapper_GroupManagment()
    {
        // Mapping from DTO_CreateGroup to Group model, setting creation time, name, description, and deletion status.
        CreateMap<DTO_CreateGroup, Group>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GroupDescription))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        // Mapping from GroupMember model to DTO_GroupType, mapping group ID and name.
        CreateMap<GroupMember, DTO_GroupType>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

        // Mapping from a list of GroupMember models to DTO_GroupsTypes, differentiating groups joined and created.
        CreateMap<List<GroupMember>, DTO_GroupsTypes>()
            .ForMember(dest => dest.GroupsJoined, opt => opt.MapFrom(src => MapToDTOGroupType(src, false)))
            .ForMember(dest => dest.GroupsCreated, opt => opt.MapFrom(src => MapToDTOGroupType(src, true)));

        // Mapping from GroupInvitation model to DTO_RecivedInvitations, including invitation message, group details, and inviting user details.
        CreateMap<GroupInvitation, DTO_RecivedInvitations>()
            .ForMember(dest => dest.InvitationMessage, opt => opt.MapFrom(src => src.InvitationMessage))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.InvitingUserId, opt => opt.MapFrom(src => Guid.Parse(src.InvitingUserId)))
            .ForMember(dest => dest.InvitingUserName, opt => opt.MapFrom(src => src.InvitingUser.FullName));

        // Mapping from GroupInvitation model to DTO_SentInvitations, including group details, invitation status, and invited user details.
        CreateMap<GroupInvitation, DTO_SentInvitations>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.InvitationTimeSend, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.InvitationStatus, opt => opt.MapFrom(src => src.InvitationStatus))
            .ForMember(dest => dest.InvitedUserId, opt => opt.MapFrom(src => src.InvitedUserId))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.InvitedUser.FullName))
            .ForMember(dest => dest.InvitationId, opt => opt.MapFrom(src => src.Id));
    }
    /// <summary>
    /// Maps a list of GroupMember objects to a list of DTO_GroupType objects based on the ownership status.
    /// </summary>
    /// <param name="groupMembers">The list of GroupMember objects to map.</param>
    /// <param name="isOwner">Boolean indicating whether to map only the groups where the user is an owner.</param>
    /// <returns>A list of DTO_GroupType objects.</returns>
    private static List<DTO_GroupType>
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
                GroupName = x.Group.Name,
                Balance = x.Balance,
            })
            .ToList();
    }
}