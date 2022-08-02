using System.ComponentModel.DataAnnotations;
using HeroesCup.Localization;

namespace HeroesCup.Web.ClubsModule.Attributes;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] extensions;

    public AllowedExtensionsAttribute(string[] extensions)
    {
        this.extensions = extensions;
    }

    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        var files = value as IEnumerable<IFormFile>;
        var validationResult = ValidationResult.Success;
        if (files != null)
        {
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName);
                validationResult = GetValidationResult(validationContext, file);
                if (validationResult != ValidationResult.Success) return validationResult;
            }
        }
        else
        {
            var file = value as IFormFile;
            if (file != null)
            {
                validationResult = GetValidationResult(validationContext, file);
                if (validationResult != ValidationResult.Success) return validationResult;
            }
        }

        return validationResult;
    }

    private ValidationResult GetValidationResult(ValidationContext validationContext, IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);

        if (!extensions.Contains(extension.ToLower()))
            return new ValidationResult(
                string.Format(GetErrorMessage(validationContext), string.Join(", ", extensions)));

        return ValidationResult.Success;
    }

    private string GetErrorMessage(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ErrorMessage)) return "Invalid error message";

        var localizer = validationContext.GetService(typeof(ManagerLocalizer)) as ManagerLocalizer;
        return localizer.General[ErrorMessage];
    }
}