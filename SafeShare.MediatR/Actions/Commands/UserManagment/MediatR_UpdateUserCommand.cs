using MediatR;
using Microsoft.AspNetCore.Mvc;
using SafeShare.DataTransormObject.UserManagment;

namespace SafeShare.MediatR.Actions.Commands.UserManagment;
public class MediatR_UpdateUserCommand : IRequest<ObjectResult>
{
    public Guid Id { get; set; }
    public DTO_UserInfo DTO_UserInfo { get; set; }

    public MediatR_UpdateUserCommand
    (
        Guid id,
        DTO_UserInfo dTO_UserInfo
    )
    {
        Id = id;
        DTO_UserInfo = dTO_UserInfo;
    }
}