using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SocialStoriesBackend.Entities;

public class User : IdentityUser<Guid>
{
    [Required]
    [Key]
    public override Guid Id { get; set; }
    
    public List<Story> Stories { get; set; }
}

