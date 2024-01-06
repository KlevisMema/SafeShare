using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SafeShare.ClientDTO.Validators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class NoXssAttribute : ValidationAttribute
{
    private static readonly string[] InvalidPatterns = {
        @"<script\b[^>]*>(.*?)<\/script>",
        @"on\w+\s*=",
        @"javascript\s*:",
        @"<\s*img[^>]*\s*src\s*=\s*['""]?[^'""]+['""]?\s*[^>]*>",
    };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string inputValue = value.ToString();

            foreach (string pattern in InvalidPatterns)
            {
                if (Regex.IsMatch(inputValue, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    return new ValidationResult("Please enter a valid input.");
                }
            }
        }

        return ValidationResult.Success;
    }
}