using SocialStoriesBackend.Mappings;
using Swashbuckle.AspNetCore.Filters;

namespace SocialStoriesBackend.Examples;

public class PageDtoExample : IExamplesProvider<StoryPageDto>
{
    public StoryPageDto GetExamples()
    {
        return new StoryPageDto
        {
            ImageId = Guid.NewGuid(),
            ImageExtension = ".jpg",
            Description = "This is a story page"
        };
    }
}
