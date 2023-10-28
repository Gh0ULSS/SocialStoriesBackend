using Microsoft.AspNetCore.Mvc.Filters;

namespace SocialStoriesBackend.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateModelAttribute : ActionFilterAttribute
{ 
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new ValidationFailedResult(context.ModelState);
        }
    }
}