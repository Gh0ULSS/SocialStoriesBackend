namespace SocialStoriesBackend.Attributes;

public static class SchemaHelper
{
    public static string DefaultSchemaIdSelector(Type modelType)
    {
        if (!modelType.IsConstructedGenericType) return modelType.Name.Replace("[]", "Array");

        var prefix = modelType.GetGenericArguments().Select(DefaultSchemaIdSelector)
            .Aggregate((previous, current) => previous + current);
        return prefix + modelType.Name.Split('`').First();
    }
}


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class SwaggerSchemaIdAttribute : Attribute
{
    public string SchemaId { get; init; }

    public SwaggerSchemaIdAttribute(string schemaId)
    {
        SchemaId = schemaId;
    }
}