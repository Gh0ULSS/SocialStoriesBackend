using System;
using System.IdentityModel.Tokens.Jwt;
using SocialStoriesBackend.DbContext;
using SocialStoriesBackend.Entities;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Models.Users.PostRequests;

using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialStoriesBackend.Mappings;
using SocialStoriesBackend.Services;


namespace SocialStoriesBackend.Controllers;

/// <summary>
/// Controller to handle User authorization 
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : BaseController<AuthController>
{
    public AuthController(
        ILogger<AuthController> logger,
        DbContextService dbContextService,
        IMapper mapper,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager)
        : base(logger, dbContextService, mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
    
    /// <summary>
    /// Endpoint to facilitate login 
    /// </summary>
    [HttpPost ("Login")]
    [ValidateModel]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {   
            _logger.LogDebug($"Failed to find user with Email: {request.Email}");
            return NotFound(new ErrorDto{ Message = new []
            {
                "User is not found on this server."
            }});
        }
        
        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!loginResult.Succeeded) {
            _logger.LogDebug($"Invalid login attempt");
            return BadRequest(new ErrorDto{ Message = new []
            {
                "Email or password is incorrect !"
            }});
        }
        
        var options = new IdentityOptions();
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
            new(options.ClaimsIdentity.UserNameClaimType, user.UserName),
        };
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);
        
        claims.AddRange(userClaims);
        
        foreach (var userRole in userRoles) {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
            var role = await _roleManager.FindByNameAsync(userRole);
            if (role is null)
                continue;

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            claims.AddRange(roleClaims);
        }
        
        var accessToken = new JwtSecurityToken(issuer: Authorization.Issuer,
                                               audience: Authorization.Audience,
                                               claims: claims,
                                               notBefore: DateTime.UtcNow,
                                               expires: DateTime.UtcNow.AddHours(1),
                                               signingCredentials: Authorization.SecurityCredentials);
        
        return Ok(new SuccessDto { Message = new[]
        {
            new JwtSecurityTokenHandler().WriteToken(accessToken),
            $"Valid from {accessToken.ValidFrom.ToUniversalTime()} UTC to {accessToken.ValidTo.ToUniversalTime()} UTC"
        }});
    }
    
    /// <summary>
    /// Endpoint to facilitate registration 
    /// </summary>
    [HttpPost ("RegisterUser")]
    [ValidateModel]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] RegisterUserRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            var errorMessage = $"Failed to add user with Username: {request.Username}/Email: {request.Email}";
            _logger.LogDebug(errorMessage);
            return Conflict(new ErrorDto{ Message = new []
            {
                errorMessage,
                "User already exists"
            }});
        }

        var newUser = new User {
            UserName = request.Username,
            Email = request.Email,
            PhoneNumber = request.MobileNumber
        };
        
        var createUserResult  = await _userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded)
        {   
            _logger.LogDebug("Failed to add a new user with username: {}", request.Username);
            
            var errorResponse = new ErrorDto
            {
                Message = new[]
                {
                    $"Failed to register {request.Username}"
                }
            };

            errorResponse.Message = errorResponse.Message.Concat(createUserResult.Errors.Select(error => error.Description).ToArray()).ToArray();
            return BadRequest(errorResponse);
        }
        
        var addUserRoleResult = await _userManager.AddToRoleAsync(newUser, Authorization.UserRole);
        if (!addUserRoleResult.Succeeded)
        {   
            _logger.LogDebug("Failed to add a new user to role: {} -> {}", request.Username, Authorization.UserRole);
            return BadRequest(new ErrorDto{ Message = new []
            {
                $"Failed to add {request.Username} to {Authorization.UserRole} role"
            }});
        }
        
        return Ok(new SuccessDto { Message = new[]
        {
            $"{request.Username} was successfully registered"
        }});
    }
    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
}