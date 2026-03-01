namespace CaseAgent.Model.Responses;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Valid() => new() { IsValid = true };

    public static ValidationResult Invalid(List<string> errors) => new()
    {
        IsValid = false,
        Errors = errors
    };
}
