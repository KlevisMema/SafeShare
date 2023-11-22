using SafeShare.ClientDTO.Authentication;

namespace SafeShare.Client.Helpers;

internal class AppState
{
    private ClientDto_LoginResult? ClientSecrests { get; set; }

    public void setClientSecrests
    (
        ClientDto_LoginResult? clientSecrest
    )
    {
        ClientSecrests = clientSecrest;
    }

    public ClientDto_LoginResult? getClientSecrests()
    {
        return ClientSecrests;
    }
}