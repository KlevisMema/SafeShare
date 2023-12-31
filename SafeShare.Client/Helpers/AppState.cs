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

    public event Action<ClientDto_GroupType?> OnNewGroupCreated;

    public void NewGroupAdded(ClientDto_GroupType? groupType) => OnNewGroupCreated.Invoke(groupType);
}