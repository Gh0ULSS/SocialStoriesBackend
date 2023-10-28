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

namespace SocialStoriesBackend.Controllers.Internal;

/// <summary>
/// Controller used to setup backend once.
/// </summary>
[ApiController]
[Route("api/internal/[controller]")]
public class SetupController : BaseController<SetupController>
{
    public SetupController(
        ILogger<SetupController> logger,
        DbContextService dbContextService,
        IMapper mapper,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager)
        : base(logger, dbContextService, mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    /// <summary>
    /// Endpoint to initialize backend and create first Admin user 
    /// </summary>
    [HttpPost("Initialize")]
    [ValidateModel]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Initialize([FromBody] RegisterUserRequest request) {
        // check if any users are admin first
        var adminRoleExists = await _roleManager.RoleExistsAsync(Authorization.AdminRole);
        if (adminRoleExists) {
            var users = await _userManager.GetUsersInRoleAsync(Authorization.AdminRole);
            if (users.Count > 0) {
                return Unauthorized(new ErrorDto {Message = new[]
                    {
                        "Admin user already exists."
                    }
                });
            }
        }
        
        // Create roles
        var roles = new[] {
            Authorization.UserRole,
            Authorization.AdminRole
        };

        foreach (var role in roles) {
            if (!await CreateRole(role)) {
                return BadRequest(new ErrorDto {Message = new[]
                    {
                        $"Failed to create role: {role}"
                    }
                });
            }
        }
        
        var newUser = new User { UserName = request.Username, Email = request.Email};
        var createUserResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded) {
            
            var errorResponse = new ErrorDto
            {
                Message = new[]
                {
                    $"Failed to create user: {request.Username}"
                }
            };

            errorResponse.Message = errorResponse.Message.Concat(createUserResult.Errors.Select(error => error.Description).ToArray()).ToArray();
            return BadRequest(errorResponse);
        }
        
        var adminRole = await _roleManager.FindByNameAsync(Authorization.AdminRole);
        if (adminRole is null) {
            return BadRequest(new ErrorDto {Message = new[]
                {
                    $"Failed to find role: {Authorization.AdminRole}"
                }
            });
        }
        
        var adminRoleResult = await _userManager.AddToRoleAsync(newUser, Authorization.AdminRole);
        if (!adminRoleResult.Succeeded) {
            var errorResponse = new ErrorDto
            {
                Message = new[]
                {
                    $"Failed to add {Authorization.AdminRole} role to user: {request.Username}"
                }
            };

            errorResponse.Message = errorResponse.Message.Concat(adminRoleResult.Errors.Select(error => error.Description).ToArray()).ToArray();
            return BadRequest(errorResponse);
        }

        return Ok(new SuccessDto { Message = new []
            {
                "Initialized backend!"
            }
        });
    }

    private async Task<bool> CreateRole(string name) {
        var roleExists = await _roleManager.RoleExistsAsync(name);
        if (roleExists)
            return true;

        var role = new IdentityRole<Guid> {
            Name = name
        };

        var result = await _roleManager.CreateAsync(role);
        return result.Succeeded;
    }
    
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
}