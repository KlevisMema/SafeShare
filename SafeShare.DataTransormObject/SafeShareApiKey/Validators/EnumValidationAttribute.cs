using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace SafeShare.DataTransormObject.SafeShareApiKey.Validators;

public class EnumValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        Type enumType = value.GetType();
        bool isValid = enumType.IsEnum && Enum.IsDefined(enumType, value);

        return isValid
            ? ValidationResult.Success
            : new ValidationResult($"Invalid value for enum {enumType.Name}");
    }
}