using System.ComponentModel.DataAnnotations;

namespace SocialStoriesBackend.Models.Users.PostRequests;

/// <summary>
/// Json object sent in body to fetch a specific users information
/// </summary>
public class FetchUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}