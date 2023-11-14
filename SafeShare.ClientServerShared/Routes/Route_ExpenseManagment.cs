namespace SafeShare.ClientServerShared.Routes;

public static class Route_ExpenseManagment
{
    public const string GetExpense = "GetExpense/{userId}";
    public const string DeleteExpense = "DeleteExpense/{userId}";
    public const string EditExpense = "EditExpense/{userId}";
    public const string CreateExpense = "CreateExpense/{userId}";
    public const string GetAllExpensesOfGroup = "GetAllExpensesOfGroup/{userId}";

}