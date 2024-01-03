using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientDTO.GroupManagment;

namespace SafeShare.Client.Helpers;

public class AppState
{
    private ClientDto_LoginResult? ClientSecrests { get; set; }


    public void
    SetClientSecrests
    (
        ClientDto_LoginResult? clientSecrest
    )
    {
        ClientSecrests = clientSecrest;
    }

    public ClientDto_LoginResult?
    GetClientSecrests()
    {
        return ClientSecrests;
    }

    public event Action<ClientDto_GroupType?>? OnNewGroupCreated;
    public event Action<ClientDto_GroupType?>? OnGroupEdited;
    public event Action<ClientDto_GroupType?>? OnGroupInvitationAccepted;
    public event Action<Guid>? OnGroupDeleted;

    public void NewGroupAdded(ClientDto_GroupType? groupType) => OnNewGroupCreated?.Invoke(groupType);
    public void GroupEdited(ClientDto_GroupType? groupType) => OnGroupEdited?.Invoke(groupType);
    public void GroupDeleted(Guid groupId) => OnGroupDeleted?.Invoke(groupId);
    public void GroupInvitationAccepted(ClientDto_GroupType clientDto_GroupType) => OnGroupInvitationAccepted?.Invoke(clientDto_GroupType);
}