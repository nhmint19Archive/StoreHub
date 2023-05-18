using System.ComponentModel.DataAnnotations;

namespace Assignment3.Application.Services;

public static class ValidationHelper
{
    /// <summary>
    /// Validates all properties of an object using validation attributes
    /// </summary>
    /// <param name="instance">Object to validate.</param>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <returns>A list of error strings if errors exist.</returns>
    public static IReadOnlyCollection<string> ValidateObject<T>(T instance) where T : notnull
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);
        return validationResults.Select(x => x.ErrorMessage ?? "Validation failed").ToList();
    }
}
