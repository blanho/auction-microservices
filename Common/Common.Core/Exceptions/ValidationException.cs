namespace Common.Core.Exceptions;

/// <summary>
/// Exception for validation errors (e.g., from FluentValidation or manual checks).
/// Contains dictionary of field-level errors.
/// </summary>
public sealed class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures occurred.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    public ValidationException(string propertyName, string errorMessage)
        : this(new Dictionary<string, string[]>
        {
            [propertyName] = new[] { errorMessage }
        })
    {
    }
}
