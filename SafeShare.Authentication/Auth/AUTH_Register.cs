using AutoMapper;
using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SafeShare.Utilities.Responses;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;
using SafeShare.DataTransormObject.Authentication;
using SafeShare.Authentication.Interfaces;

namespace SafeShare.Authentication.Auth;

public class AUTH_Register : IAUTH_Register
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AUTH_Register> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AUTH_Register
    (
        IMapper mapper,
        ApplicationDbContext db,
        ILogger<AUTH_Register> logger,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager
    )
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Util_GenericResponse<bool>>
    RegisterUser
    (
        DTO_Register registerDto
    )
    {
        try
        {
            var test = _mapper.Map<ApplicationUser>(registerDto);

            var createUserResult = await _userManager.CreateAsync(test, registerDto.Password);

            if (!createUserResult.Succeeded)
                return Util_GenericResponse<bool>.Response(false, false, "Something went wrong when creating the user!", createUserResult.Errors.Select(x => x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);

            var assignRole = await AssignUserToUserRole(registerDto.UserName);

            if (!assignRole.Succsess)
                return assignRole;

            _logger.LogInformation($"[BLL Module] - [RegisterUser Method], {Util_GetIpAddres.GetLocation(_httpContextAccessor)} user {@createUserResult} was succsessfully created created.");

            return Util_GenericResponse<bool>.Response(true, true, "Your account was successfully created", null, System.Net.HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            var errorResponse = Util_GenericResponse<bool>.Response(false, false, ex.ToString(), null, System.Net.HttpStatusCode.InternalServerError);

            _logger.LogError(ex, $"Somewthing went wrong in [Authentication Module] - [RegisterUser Method], user with Ip {Util_GetIpAddres.GetLocation(_httpContextAccessor)}", errorResponse);

            return errorResponse;
        }
    }

    private async Task<Util_GenericResponse<bool>>
    AssignUserToUserRole
    (
        string userName
    )
    {
        try
        {
            var user = await _userManager.FindByNameAsync(userName);

            var roles = await _db.Roles.ToListAsync();

            if (roles.Count > 0 && user is not null)
            {
                string? userRole = roles?.FirstOrDefault(role => role.Name == "User")?.Name;

                if (String.IsNullOrEmpty(userRole))
                {
                    await _userManager.DeleteAsync(user);
                    return Util_GenericResponse<bool>.Response(false, false, "Something went wrong in assigning the role to the user, user was not created!", null, System.Net.HttpStatusCode.BadRequest);
                }

                var addToRole = await _userManager.AddToRoleAsync(user!, userRole);

                if (addToRole is null)
                {
                    await _userManager.DeleteAsync(user);
                    return Util_GenericResponse<bool>.Response(false, false, "Something went wrong in assigning the role to the user, user was not created!", null, System.Net.HttpStatusCode.BadRequest);
                }
            }

            return Util_GenericResponse<bool>.Response(true, true, "User succsessfully assigned to user role", null, System.Net.HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            var errorResponse = Util_GenericResponse<bool>.Response(false, false, ex.ToString(), null, System.Net.HttpStatusCode.InternalServerError);

            _logger.LogError(ex, $"Somewthing went wrong in [Authentication Module] - [AssignUserToUserRole Method], user with Ip {Util_GetIpAddres.GetLocation(_httpContextAccessor)}", errorResponse);

            return errorResponse;
        }
    }
}