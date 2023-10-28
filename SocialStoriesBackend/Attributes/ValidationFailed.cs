using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SocialStoriesBackend.Attributes;

public class ValidationResultModel
{
    public List<string> Message { get; }

    public ValidationResultModel(ModelStateDictionary modelState)
    {
        Message = modelState.Keys.SelectMany(key => modelState[key]?.Errors.Select(x => x.ErrorMessage) ?? Array.Empty<string>()).ToList();
    }
}

public class ValidationFailedResult : ObjectResult
{
    public ValidationFailedResult(ModelStateDictionary modelState) : base(new ValidationResultModel(modelState))
    {
        StatusCode = StatusCodes.Status400BadRequest;
    }
}