using SafeShare.ClientDTO.Expense;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientDTO.GroupManagment;

namespace SafeShare.Client.Internal;

public class AppState
{
    private ClientDto_LoginResult? ClientSecrests { get; set; }


    /// <summary>
    /// A property indicating that user have public key in the database but no keys in his 
    /// browser this means that user is logging form a new device or has cleared his browser so 
    /// his keys needs to be generated again and saved in the browser.
    /// </summary>
    public bool GenerateSameKeys { get; set; } = false;

    public void
    SetClientSecrets
    (
        ClientDto_LoginResult? clientSecrest
    )
    {
        ClientSecrests = clientSecrest;
    }

    public ClientDto_LoginResult?
    GetClientSecrets()
    {
        return ClientSecrests;
    }

    public event Action? OnLogOut;
    public event Action<Guid>? OnGroupDeleted;
    public event Action<Guid>? OnRemovedFromGroup;
    public event Action<ClientDto_Expense>? OnExpenseEditted;
    public event Action<ClientDto_GroupType?>? OnGroupEdited;
    public event Action<ClientDto_GroupDetails?>? OnGroupDetails;
    public event Action<ClientDto_GroupType?>? OnNewGroupCreated;
    public event Action<decimal>? OnExpenseCreatedOnGroupsJoined;
    public event Action<decimal>? OnExpenseCreatedOnGroupsCreated;
    public event Action<ClientDto_GroupType?>? OnGroupInvitationAccepted;

    public void LogOut() => OnLogOut?.Invoke();
    public void GroupDeleted(Guid groupId) => OnGroupDeleted?.Invoke(groupId);
    public void RemovedFromGroup(Guid groupId) => OnRemovedFromGroup?.Invoke(groupId);
    public void ExpenseEdited(ClientDto_Expense expense) => OnExpenseEditted?.Invoke(expense);
    public void GroupEdited(ClientDto_GroupType? groupType) => OnGroupEdited?.Invoke(groupType);
    public void NewGroupAdded(ClientDto_GroupType? groupType) => OnNewGroupCreated?.Invoke(groupType);
    public void GroupDetails(ClientDto_GroupDetails? groupDetails) => OnGroupDetails?.Invoke(groupDetails);
    public void ExpenseCreatedOnGroupsJoined(decimal amount) => OnExpenseCreatedOnGroupsJoined?.Invoke(amount);
    public void ExpenseCreatedOnGroupsCreated(decimal amount) => OnExpenseCreatedOnGroupsCreated?.Invoke(amount);
    public void GroupInvitationAccepted(ClientDto_GroupType clientDto_GroupType) => OnGroupInvitationAccepted?.Invoke(clientDto_GroupType);
}