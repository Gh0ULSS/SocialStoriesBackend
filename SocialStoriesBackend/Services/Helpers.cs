namespace SocialStoriesBackend.Services;

public static class Helpers
{
    public static bool InDocker()
    {
        var envVariable = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
        return envVariable is not null && envVariable == "true";
    }

    public static string StripFileExtension(string fileExtension)
    {
        char[] charsToTrim = { '*', ' ', '\'', '.', ',', '\"'};
        return fileExtension.Trim(charsToTrim);
    }
    
    public static string? ToBase64(byte[] data) {
        return data.Length <= 0 ? null : Convert.ToBase64String(data);
    }

    public static byte[]? TryParseBase64(string encodedText) {
        if (encodedText.Length <= 0) {
            return null;
        }

        try {
            return Convert.FromBase64String(encodedText);
        }
        catch (Exception) {
            return null;
        }
    }
}