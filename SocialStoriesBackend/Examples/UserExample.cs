using SocialStoriesBackend.Mappings;
using Swashbuckle.AspNetCore.Filters;

namespace SocialStoriesBackend.Examples;

public class UserExample : IExamplesProvider<UserDto>
{
    public UserDto GetExamples()
    {
        return new UserDto
        {
            Email = "Admin@sunsetstories.com",
            Id = Guid.NewGuid(),
            Username = "Admin"
        };
    }
}