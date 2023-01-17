using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TrafficCourts.Staff.Service.Models;

/// <summary>
/// Represents a single file to upload to COMS and its metadata
/// </summary>
public class FileUploadRequest
{
    // TODO: Determine the specific file format mime types allowed from the request
    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    [FromForm]
    [ModelBinder(BinderType = typeof(DictionaryBinder<string, string>))]
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// Custom IModelBinder for Dictionary to get parameters from request into the model
/// This method code has been taken from the following resource.
/// Resource: https://stackoverflow.com/a/67230419
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DictionaryBinder<TKey, TValue> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        if (bindingContext.HttpContext.Request.HasFormContentType)
        {
            var form = bindingContext.HttpContext.Request.Form;
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            var data = JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(form[bindingContext.FieldName]!);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
            bindingContext.Result = ModelBindingResult.Success(data);
        }

        return Task.CompletedTask;
    }
}
