using SocialStoriesBackend.Models.Users.PostRequests;
using Swashbuckle.AspNetCore.Filters;

namespace SocialStoriesBackend.Examples;

public class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples()
    {
        return new LoginRequest
        {
            Email = "Admin@sunsetstories.com",
            Password = "Password12!"
        };
    }
}

public class RegisterUserRequestExample : IExamplesProvider<RegisterUserRequest>
{
    public RegisterUserRequest GetExamples()
    {
        return new RegisterUserRequest
        {
            Email = "Admin@sunsetstories.com",
            Username = "Admin",
            MobileNumber = "0400000000",
            Password = "Password12!"
        };
    }
}