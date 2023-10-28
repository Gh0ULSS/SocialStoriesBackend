using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace SocialStoriesBackend.Entities;

public class StoryPage
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid ImageId { get; set; }
    
    [Required]
    public string ImageExtension { get; set; }

    [Required] 
    public string Description { get; set; }
}