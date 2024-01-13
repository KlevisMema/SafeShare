using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SafeShare.ClientServerShared.Routes;
using Microsoft.AspNetCore.Http.HttpResults;
using SafeShare.Security.JwtSecurity.Helpers;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.Utilities.SafeShareApi.Responses;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.DataTransormObject.SafeShareApi.UserManagment;
using SafeShare.DataTransormObject.SafeShareApi.ExpenseManagment;

namespace SafeShare.ProxyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Default")]
public class ExpenseManagmentProxyController(IExpenseManagmentProxyService expenseManagmentProxyService) : ControllerBase
{
    [HttpGet(Route_ExpenseManagment.ProxyGetAllExpensesOfGroup)]
    public async Task<ActionResult<Util_GenericResponse<List<DTO_Expense>>>>
    GetAllExpensesOfGroup
    (
       [FromQuery] Guid groupId
    )
    {
        var jwtToken = JwtToken();

        var result = await expenseManagmentProxyService.GetAllExpensesOfGroup(UserId(jwtToken), jwtToken, groupId);

        return Util_GenericControllerResponse<List<DTO_Expense>>.ControllerResponse(result);
    }

    [HttpGet(Route_ExpenseManagment.ProxyGetExpense)]
    public async Task<ActionResult<Util_GenericResponse<DTO_Expense>>>
    GetExpense
    (
        Guid expenseId
    )
    {
        var jwtToken = JwtToken();

        var result = await expenseManagmentProxyService.GetExpense(UserId(jwtToken), jwtToken, expenseId);

        return Util_GenericControllerResponse<DTO_Expense>.ControllerResponse(result);
    }

    [HttpPost(Route_ExpenseManagment.ProxyCreateExpense)]
    public async Task<ActionResult<Util_GenericResponse<DTO_Expense>>>
    CreateExpense
    (
        [FromForm] DTO_ExpenseCreate expenseDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await expenseManagmentProxyService.CreateExpense(UserId(jwtToken), jwtToken, expenseDto);

        return Util_GenericControllerResponse<DTO_Expense>.ControllerResponse(result);
    }

    [HttpPut(Route_ExpenseManagment.ProxyEditExpense)]
    public async Task<ActionResult<Util_GenericResponse<DTO_Expense>>>
    EditExpense
    (
        Guid expenseId,
        [FromForm] DTO_ExpenseCreate expenseCreateDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await expenseManagmentProxyService.EditExpense(UserId(jwtToken), jwtToken, expenseId, expenseCreateDto);

        return Util_GenericControllerResponse<DTO_Expense>.ControllerResponse(result);
    }

    [HttpDelete(Route_ExpenseManagment.ProxyDeleteExpense)]
    public async Task<ActionResult<Util_GenericResponse<bool>>>
    DeleteExpense
    (
        [FromBody] DTO_ExpenseDelete expenseDelete
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jwtToken = JwtToken();

        var result = await expenseManagmentProxyService.DeleteExpense(UserId(jwtToken), jwtToken, expenseDelete);

        return Util_GenericControllerResponse<bool>.ControllerResponse(result);
    }

    private static string
    UserId
    (
        string jwtToken
    )
    {
        return Helper_JwtToken.GetUserIdDirectlyFromJwtToken(jwtToken);
    }

    private string
    JwtToken()
    {
        return Request.Cookies["AuthToken"] ?? string.Empty;
    }
}