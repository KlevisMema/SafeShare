using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientDTO.Expense;
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

    public event Action<Guid>? OnGroupDeleted;
    public event Action<ClientDto_Expense>? OnExpenseEditted;
    public event Action<ClientDto_GroupType?>? OnGroupEdited;
    public event Action<ClientDto_GroupDetails?>? OnGroupDetails;
    public event Action<ClientDto_GroupType?>? OnNewGroupCreated;
    public event Action<decimal>? OnExpenseCreatedOnGroupsJoined;
    public event Action<decimal>? OnExpenseCreatedOnGroupsCreated;
    public event Action<ClientDto_GroupType?>? OnGroupInvitationAccepted;

    public void GroupDeleted(Guid groupId) => OnGroupDeleted?.Invoke(groupId);
    public void ExpenseEditted(ClientDto_Expense expense) => OnExpenseEditted?.Invoke(expense);
    public void GroupEdited(ClientDto_GroupType? groupType) => OnGroupEdited?.Invoke(groupType);
    public void NewGroupAdded(ClientDto_GroupType? groupType) => OnNewGroupCreated?.Invoke(groupType);
    public void GroupDetails(ClientDto_GroupDetails? groupDetails) => OnGroupDetails?.Invoke(groupDetails);
    public void ExpenseCreatedOnGroupsJoined(decimal amount) => OnExpenseCreatedOnGroupsJoined?.Invoke(amount);
    public void ExpenseCreatedOnGroupsCreated(decimal amount) => OnExpenseCreatedOnGroupsCreated?.Invoke(amount);
    public void GroupInvitationAccepted(ClientDto_GroupType clientDto_GroupType) => OnGroupInvitationAccepted?.Invoke(clientDto_GroupType);
}