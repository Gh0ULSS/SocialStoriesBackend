using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Entities;

namespace SocialStoriesBackend.Mappings;

public class TemplateStoryProfile : Profile
{
    public TemplateStoryProfile()
    {
        CreateMap<TemplateStory, TemplateStoryDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Type,
                opt => opt.MapFrom(src => src.Type)
            ).ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(src => src.Title)
            )
            .ForMember(
                dest => dest.PageDescriptions,
                opt => opt.MapFrom(src => src.PageDescriptions)
            )
            .ReverseMap();
    }
}

/// <summary>
/// Json object representing a user template story
/// </summary>
[SwaggerSchemaId("TemplateStory")]
public class TemplateStoryDto
{
    /// <summary>
    /// Story template ID
    /// </summary>
    [Required]
    public Guid Id { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Story template type
    /// </summary>
    [Required] 
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Story template title
    /// </summary>
    [Required] 
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// List of template page descriptions with placeholder markers
    /// </summary>
    [Required] 
    public List<string> PageDescriptions { get; set; } = new List<string>();
}