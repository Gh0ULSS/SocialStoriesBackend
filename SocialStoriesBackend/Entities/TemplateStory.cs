using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SocialStoriesBackend.Entities;

public class TemplateStory
{
    [Required]
    [Key]
    public Guid Id { get; set; }
    
    [Required] 
    public string Type { get; set; }
    
    [Required] 
    public string Title { get; set; }
    
    [Required]
    public List<string> PageDescriptions { get; set; }
}