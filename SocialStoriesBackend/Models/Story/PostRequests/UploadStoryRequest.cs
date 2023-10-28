using System.ComponentModel.DataAnnotations;
using SocialStoriesBackend.Mappings;

namespace SocialStoriesBackend.Models.Story.PostRequests;

/// <summary>
/// Json object sent in body to fetch a specific users information
/// </summary>
public class UploadStoryRequest
{
    /// <summary>
    /// Story title page
    /// </summary>
    [Required]
    public StoryPageDto TitleStoryPage { get; set; }

    /// <summary>
    /// Story page count
    /// </summary>
    [Required]
    public int PageCount { get; set; }

    /// <summary>
    /// An array of story pages
    /// </summary>
    [Required]
    public List<StoryPageDto> Pages { get; set; }
    
    /// <summary>
    /// Font style
    /// </summary>
    [Required]
    public string FontType { get; set; }
}