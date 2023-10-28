using System.ComponentModel.DataAnnotations;
using SocialStoriesBackend.Attributes;

namespace SocialStoriesBackend.Mappings;

/// <summary>
/// Json object returned when an operation succeeds
/// </summary>
[SwaggerSchemaId("Success")]
public class SuccessDto
{
    /// <summary>
    /// An array of informative strings
    /// </summary>
    [Required]
    public string[] Message { get; set; } = Array.Empty<string>();
}