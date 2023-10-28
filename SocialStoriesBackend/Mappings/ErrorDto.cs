using System.ComponentModel.DataAnnotations;
using SocialStoriesBackend.Attributes;

namespace SocialStoriesBackend.Mappings;

/// <summary>
/// Json object returned when an error occurs 
/// </summary>
[SwaggerSchemaId("Error")]
public class ErrorDto
{
    /// <summary>
    /// An array of error strings
    /// </summary>
    [Required]
    public string[] Message { get; set; } = Array.Empty<string>();
}