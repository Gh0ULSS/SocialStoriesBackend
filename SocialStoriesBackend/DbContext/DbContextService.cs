using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SocialStoriesBackend.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SocialStoriesBackend.DbContext;

public class DbContextService : IdentityDbContext<User, IdentityRole<Guid>, Guid> {
    public DbContextService(DbContextOptions<DbContextService> options) : base(options) { }
    
    public DbSet<Story> Stories { get; set; }
    public DbSet<TemplateStory> TemplateStories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
        
        // User
        // Note: the rest of the DB columns are automatically handled by IdentityUser
        builder.Entity<User>()
               .HasKey(x => x.Id);
            
        builder.Entity<User>()
               .HasMany<Story>(x => x.Stories)
               .WithOne(x => x.User);
        
        // Page
        builder.Entity<StoryPage>()
            .HasKey(x => x.Id);

        // Story
        builder.Entity<Story>()
               .HasKey(x => x.Id);
        
        builder.Entity<Story>()
            .HasOne<User>(x => x.User)
            .WithMany(x => x.Stories);

        builder.Entity<Story>()
            .HasOne<StoryPage>(x => x.TitleStoryPage);

        builder.Entity<Story>()
            .HasMany<StoryPage>(x => x.Pages);
        
        // Template stories
        builder.Entity<TemplateStory>()
               .HasKey(x => x.Id);
        
        builder.Entity<TemplateStory>().Property(p => p.PageDescriptions)
            .HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>());
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
