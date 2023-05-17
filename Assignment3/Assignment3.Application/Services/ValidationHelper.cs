using System.ComponentModel.DataAnnotations;

namespace Assignment3.Application.Services;

public static class ValidationHelper
{
    public static IReadOnlyCollection<string> ValidateObject<T>(T instance) where T : notnull
    {
        var results = new List<ValidationResult>();
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);
        return validationResults.Select(x => x.ErrorMessage ?? "Validation failed").ToList();
    }
}
