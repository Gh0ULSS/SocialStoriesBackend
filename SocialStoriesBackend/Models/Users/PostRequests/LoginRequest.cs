using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialStoriesBackend.Models.Users.PostRequests;

/// <summary>
/// Json object sent in body to login
/// </summary>
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [PasswordPropertyText(true)]
    public string Password { get; set; } = string.Empty;
}