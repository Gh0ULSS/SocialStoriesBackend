using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Entities;

namespace SocialStoriesBackend.Mappings;

// Maps internal StoryPage object to StoryPageDTO
public class PageProfile : Profile
{
    public PageProfile()
    {
        CreateMap<StoryPage, StoryPageDto>()
            .ForMember(
                dest => dest.ImageId,
                opt => opt.MapFrom(src => src.ImageId)
            )
            .ForMember(
                dest => dest.ImageExtension,
                opt => opt.MapFrom(src => src.ImageExtension)
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Description)
            )
            .ReverseMap();
    }
}

/// <summary>
/// Json object representing a story page
/// </summary>
[SwaggerSchemaId("Page")]
public class StoryPageDto
{
    /// <summary>
    /// ID of image stored
    /// </summary>
    [Required]
    public Guid ImageId { get; set; } = Guid.Empty;
    
    /// <summary>
    /// File extension of image stored
    /// </summary>
    [Required]
    public string ImageExtension { get; set; } = string.Empty;
    
    /// <summary>
    /// Page description
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;
}