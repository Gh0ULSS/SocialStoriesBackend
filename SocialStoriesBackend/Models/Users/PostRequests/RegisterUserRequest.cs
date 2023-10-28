using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialStoriesBackend.Models.Users.PostRequests;

/// <summary>
/// Json object sent in body to register a new user
/// </summary>
public class RegisterUserRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string MobileNumber { get; set; } = string.Empty;
    
    [Required]
    [PasswordPropertyText(true)]
    public string Password { get; set; } = string.Empty;
}