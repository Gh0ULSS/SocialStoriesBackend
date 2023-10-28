using System.ComponentModel.DataAnnotations;
using SocialStoriesBackend.Attributes;

namespace SocialStoriesBackend.Mappings;

/// <summary>
/// Json object returned containing an array of user stories
/// </summary>
[SwaggerSchemaId("Stories")]
public class StoriesDto
{
    /// <summary>
    /// An array of user stories
    /// </summary>
    [Required]
    public StoryDto[] Stories { get; set; } = Array.Empty<StoryDto>();
}