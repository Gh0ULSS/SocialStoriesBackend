using SocialStoriesBackend.DbContext;
using SocialStoriesBackend.Entities;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Models.Users.PostRequests;

using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialStoriesBackend.Mappings;
using SocialStoriesBackend.Services;


namespace SocialStoriesBackend.Controllers;

/// <summary>
/// Controller providing user operations 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController<UserController>
{
    public UserController(
        ILogger<UserController> logger,
        DbContextService dbContextService,
        IMapper mapper,
        UserManager<User> userManager)
        : base(logger, dbContextService, mapper)
    {
        _userManager = userManager;
    }
    
    /// <summary>
    /// Endpoint to fetch information on a specific user
    /// </summary>
    [HttpPost ("FetchUser")]
    [ValidateModel]
    [Authorize(Policy = Authorization.UserPolicy)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] FetchUserRequest request) {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {   
            _logger.LogDebug($"Failed to find user with Email: {request.Email}");
            return NotFound(new ErrorDto{ Message = new []
            {
                "User is not found on this server."
            }});
        }
        
        return Ok(_mapper.Map<UserDto>(user));
    }
    
    private readonly UserManager<User> _userManager;
}