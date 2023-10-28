using System.ComponentModel.DataAnnotations;
using SocialStoriesBackend.Attributes;

namespace SocialStoriesBackend.Mappings;

/// <summary>
/// Json object returned containing an array of template user stories
/// </summary>
[SwaggerSchemaId("TemplateStories")]
public class TemplateStoriesDto
{
    /// <summary>
    /// An array of template user stories
    /// </summary>
    [Required]
    public TemplateStoryDto[] Stories { get; set; } = Array.Empty<TemplateStoryDto>();
}