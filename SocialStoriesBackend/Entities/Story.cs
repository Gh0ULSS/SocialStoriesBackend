using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SocialStoriesBackend.Entities;

public class Story
{
    [Required]
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    [Required]
    public string FontType { get; set; }
    
    [Required] 
    public StoryPage TitleStoryPage { get; set; }
    
    [Required] 
    public List<StoryPage> Pages { get; set; }
}