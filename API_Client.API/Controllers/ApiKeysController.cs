using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using API_Client.BLL.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SafeShare.DataTransormObject.SafeShareApiKey.ApiKey;

namespace API_Client.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Default")]
public class ApiKeysController(IApiKeyService apiKeyService) : ControllerBase
{
    [HttpGet("{clientId}")]
    public async Task<IActionResult> 
    GetApiKeys
    (
        Guid clientId
    )
    {
        var result = await apiKeyService.GetApiKeys(clientId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return NotFound(result.Message);
    }

    [HttpGet("{id}/{clientId}")]
    public async Task<IActionResult> 
    GetApiKey
    (
        Guid id, 
        Guid clientId
    )
    {
        var result = await apiKeyService.GetApiKey(id, clientId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return NotFound(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> 
    PostApiKey
    (
        [FromBody] DTO_CreateApiKey createDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await apiKeyService.PostApiKey(createDto);
        if (result.Success)
            return Ok(result);

        return BadRequest(result.Message);
    }

    [HttpPut("{id}/{clientId}")]
    public async Task<IActionResult> 
    PutApiKey
    (
        Guid id, 
        Guid clientId, 
        [FromBody] DTO_UpdateApiKey updateDto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await apiKeyService.PutApiKey(id, clientId, updateDto);
        if (result.Success)
            return Ok(result);

        return NotFound(result.Message);
    }

    [HttpDelete("{id}/{clientId}")]
    public async Task<IActionResult> 
    DeleteApiKey
    (
        Guid id, 
        Guid clientId
    )
    {
        var result = await apiKeyService.DeleteApiKey(id, clientId);

        if (result.Success)
            return Ok(result);

        return NotFound(result.Message);
    }
}