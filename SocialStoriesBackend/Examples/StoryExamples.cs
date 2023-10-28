using SocialStoriesBackend.Mappings;
using SocialStoriesBackend.Models.Story.PostRequests;
using Swashbuckle.AspNetCore.Filters;

namespace SocialStoriesBackend.Examples;

public class StoryDtoExample : IExamplesProvider<StoryDto>
{
    public StoryDto GetExamples()
    {
        var pageDtoExample = new PageDtoExample();
        var pageList = new List<StoryPageDto>()
        {
            pageDtoExample.GetExamples(),
            pageDtoExample.GetExamples(),
            pageDtoExample.GetExamples()
        };
        
        return new StoryDto
        {
            Id = Guid.NewGuid(),
            TitleStoryPage = pageDtoExample.GetExamples(),
            PageCount = pageList.Count,
            Pages = pageList,
            FontType = "Arial"
        };
    }
}

public class TemplateStoryDtoExample : IExamplesProvider<TemplateStoryDto>
{
    public TemplateStoryDto GetExamples()
    {
        
        return new TemplateStoryDto
        {
            Id = Guid.NewGuid(),
            Title = "Washing your hands",
            Type = "Hygiene",
            PageDescriptions =
            {
                "When it's time to eat, [Actor] will rush to eat",
                "Wait!, [Actor] can't eat, without washing their hands",
                "This will keep [Actor] happy and healthy",
                "[Actor] has sat down to eat, wow what a feast!",
                "[Actor] has finished their feast, now they will wash their hands again"
            }
        };
    }
}

public class TemplateStoriesDtoExample : IExamplesProvider<TemplateStoriesDto>
{
    public TemplateStoriesDto GetExamples()
    {
        var templateStoryDtoExample = new TemplateStoryDtoExample();
        var storyList = new List<TemplateStoryDto>()
        {
            templateStoryDtoExample.GetExamples(),
            templateStoryDtoExample.GetExamples(),
            templateStoryDtoExample.GetExamples()
        };
        
        return new TemplateStoriesDto
        {
            Stories = storyList.ToArray()
        };
    }
}

public class StoriesDtoExample : IExamplesProvider<StoriesDto>
{
    public StoriesDto GetExamples()
    {
        var storyDtoExample = new StoryDtoExample();
        var storyList = new List<StoryDto>()
        {
            storyDtoExample.GetExamples(),
            storyDtoExample.GetExamples(),
            storyDtoExample.GetExamples()
        };
        
        return new StoriesDto
        {
           Stories = storyList.ToArray()
        };
    }
}

public class UploadStoryRequestExample : IExamplesProvider<UploadStoryRequest>
{
    public UploadStoryRequest GetExamples()
    {
        var pageDtoExample = new PageDtoExample();
        var pageList = new List<StoryPageDto>()
        {
            pageDtoExample.GetExamples(),
            pageDtoExample.GetExamples(),
            pageDtoExample.GetExamples()
        };
        
        return new UploadStoryRequest
        {
            TitleStoryPage = pageDtoExample.GetExamples(),
            FontType = "Arial",
            PageCount = pageList.Count,
            Pages = pageList
        };
    }
}
