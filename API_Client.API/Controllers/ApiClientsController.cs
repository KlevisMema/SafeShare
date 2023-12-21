using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using API_Client.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.SafeShareApiKey.User;

namespace API_Client.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiClientsController(IApiClientService apiClientService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> 
    CreateApiClient
    (
        [FromForm] DTO_CreateApiClient createDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await apiClientService.CreateApiClient(createDto);
        if (result.Success)
            return Ok(result);

        return BadRequest(result.Message);
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = "Default")]
    public async Task<IActionResult> 
    GetApiClient
    (
        Guid id
    )
    {
        var result = await apiClientService.GetApiClient(id);

        if (result.Success)
            return Ok(result.Data);

        return NotFound(result.Message);
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = "Default")]
    public async Task<IActionResult> 
    UpdateApiClient
    (
        Guid id, 
        [FromForm] DTO_UpdateApiClient updateDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await apiClientService.UpdateApiClient(id, updateDto);

        if (result.Success)
            return Ok(result);
        
        return NotFound(result.Message);
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Default")]
    public async Task<IActionResult> 
    DeleteApiClient
    (
        Guid id
    )
    {
        var result = await apiClientService.DeleteApiClient(id);

        if (result.Success)
            return Ok(result);
        
        return NotFound(result.Message);
    }
}