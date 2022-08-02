using System.ComponentModel.DataAnnotations;
using HeroesCup.Localization;

namespace HeroesCup.Web.ClubsModule.Attributes;

public class MaxSizeFileAttribute : ValidationAttribute
{
    private readonly int maxSize;

    public MaxSizeFileAttribute(int maxSize)
    {
        this.maxSize = maxSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var files = value as IEnumerable<IFormFile>;
        if (files != null)
        {
            long size = 0;
            foreach (var file in files)
            {
                size += file.Length;
                if (size > maxSize)
                    return new ValidationResult(string.Format(GetErrorMessage(validationContext), maxSize * 0.000001));
            }
        }
        else
        {
            var file = value as IFormFile;
            if (file != null)
                if (file.Length > maxSize)
                    return new ValidationResult(string.Format(GetErrorMessage(validationContext), maxSize * 0.000001));
        }

        return ValidationResult.Success;
    }

    private string GetErrorMessage(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ErrorMessage)) return "Invalid error message";

        var localizer = validationContext.GetService(typeof(ManagerLocalizer)) as ManagerLocalizer;
        return localizer.General[ErrorMessage];
    }
}