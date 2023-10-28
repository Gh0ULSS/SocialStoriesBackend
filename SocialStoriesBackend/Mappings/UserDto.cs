using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SocialStoriesBackend.Attributes;
using SocialStoriesBackend.Entities;

namespace SocialStoriesBackend.Mappings;

// Maps internal user object to UserDTO
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Username,
                opt => opt.MapFrom(src => src.UserName)
            )
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            );
    }
}

/// <summary>
/// Json object containing user data
/// </summary>
[SwaggerSchemaId(nameof(User))]
public class UserDto
{
    
    /// <summary>
    /// Users unique GUID
    /// </summary>
    [Required]
    [Key]
    public Guid Id { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Users username
    /// </summary>
    [Required]
    [MaxLength(512)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Users email
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(512)]
    public string Email { get; set; } = string.Empty;
}