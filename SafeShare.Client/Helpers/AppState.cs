using SafeShare.ClientDTO.Authentication;

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
}