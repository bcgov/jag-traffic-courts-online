using System.ComponentModel.DataAnnotations;
namespace TrafficCourts.Citizen.Service.Validators;

/// <summary>A file validation extension ensuring the ContentType matches the permitted types.</summary>
public class PermittedFileContentType : ValidationAttribute
{
    private readonly string[] _contentTypes;
    public PermittedFileContentType(string[] contentTypes)
    {
        _contentTypes = contentTypes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null)
        {
            if (!_contentTypes.Contains(file.ContentType))
            {
                return new ValidationResult(GetErrorMessage(file.ContentType));
            }
        }

        return ValidationResult.Success;
    }

    public string GetErrorMessage(string contentType)
    {
        return string.Format($"ContentType '{contentType}' is not allowed.", contentType);
    }
}