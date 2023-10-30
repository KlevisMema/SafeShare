using SafeShare.DataAccessLayer.Models;
using SafeShare.Utilities.Responses;

namespace SafeShare.GroupManagment.Interfaces
{
    public interface IGroupManagment_GroupInvitationsRepository
    {
        Task<Util_GenericResponse<bool>> AcceptInvitation(Guid invitationId);
        List<GroupInvitation> GetReceivedInvitations(string userId);
        List<GroupInvitation> GetSentInvitations(string userId);
        void RejectInvitation(int invitationId);
        Task<Util_GenericResponse<bool>> SendInvitation(string invitingUserId, string invitedUserId, Guid groupId);
    }
}