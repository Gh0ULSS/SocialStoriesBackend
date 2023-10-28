using System.ComponentModel.DataAnnotations;
using SocialStoriesBackend.Attributes;

namespace SocialStoriesBackend.Models.File.PostRequests;

/// <summary>
/// Json object sent in body to operate on a specific file
/// </summary>
public class UploadFileRequest
{
    [Required]
    public string FileExtension { get; set; } = string.Empty;
    
    [Required]
    [Base64]
    public string EncodedData { get; set; } = string.Empty;
}