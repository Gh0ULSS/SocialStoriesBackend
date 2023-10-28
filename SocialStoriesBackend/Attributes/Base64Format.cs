using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SocialStoriesBackend.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class Base64Attribute : ValidationAttribute
{
    private static bool IsBase64String(string value) {
        return Regex.IsMatch(value, @"^[a-zA-Z0-9+/]*={0,3}$") && (value.Length % 4 == 0);
    }
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        if (value is string fieldString && !IsBase64String(fieldString)) {
            return new ValidationResult(ErrorMessage ?? "The value is not in Base64 format.");
        }

        return ValidationResult.Success;
    }
}