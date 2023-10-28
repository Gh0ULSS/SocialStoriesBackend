using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Entities;

namespace SocialStoriesBackend.Mappings;

// Maps internal page object to PageDTO
public class StoryProfile : Profile
{
    public StoryProfile()
    {
        CreateMap<Story, StoryDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.TitleStoryPage,
                opt => opt.MapFrom(src => src.TitleStoryPage)
            )
            .ForMember(
                dest => dest.PageCount,
                opt => opt.MapFrom(src => src.Pages.Count)
            )
            .ForMember(
                dest => dest.Pages,
                opt => opt.MapFrom(src => src.Pages)
            )
            .ForMember(
                dest => dest.FontType,
                opt => opt.MapFrom(src => src.FontType)
            )
            .ReverseMap();
    }
}

/// <summary>
/// Json object representing a user story
/// </summary>
[SwaggerSchemaId("Story")]
public class StoryDto
{
    /// <summary>
    /// Story ID
    /// </summary>
    [Required]
    public Guid Id { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Story title page
    /// </summary>
    [Required]
    public StoryPageDto TitleStoryPage { get; set; } = new();

    /// <summary>
    /// Story page count
    /// </summary>
    [Required]
    public int PageCount { get; set; } = 0;
    
    /// <summary>
    /// An array of story pages
    /// </summary>
    [Required]
    public List<StoryPageDto> Pages { get; set; } = new();
    
    /// <summary>
    /// Font style
    /// </summary>
    [Required]
    public string FontType { get; set; } = string.Empty;
}   