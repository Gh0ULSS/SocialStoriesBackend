using SocialStoriesBackend.Mappings;
using Swashbuckle.AspNetCore.Filters;

namespace SocialStoriesBackend.Examples;

public class SuccessExample : IExamplesProvider<SuccessDto>
{
    public SuccessDto GetExamples()
    {
        return new SuccessDto
        {
            Message = new []
            {
                "Operation was completed successfully",
                "Blah blah"
            }
        };
    }
}

public class ErrorExample : IExamplesProvider<ErrorDto>
{
    public ErrorDto GetExamples()
    {
        return new ErrorDto
        {
            Message = new []
            {
                "Operation has failed",
                "Mission Failed We'll Get Em Next Time"
            }
        };
    }
}