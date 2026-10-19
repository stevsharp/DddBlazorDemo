namespace DddBlazorDemo.SharedKernel;

/// <summary>
/// Small precondition helpers for aggregates and value objects. Every failure is a
/// <see cref="DomainException"/> so the UI can show the message as-is.
/// </summary>
public static class Guard
{
    public static T NotNull<T>(T? value, string what) where T : class
        => value ?? throw new DomainException($"{what} is required.");

    public static string NotBlank(string? value, string what)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{what} is required.");

        return value.Trim();
    }

    public static string MaxLength(string value, int max, string what)
    {
        if (value.Length > max)
            throw new DomainException($"{what} must be at most {max} characters.");

        return value;
    }

    public static Guid NotEmpty(Guid value, string what)
        => value == Guid.Empty ? throw new DomainException($"{what} is required.") : value;

    public static decimal Positive(decimal value, string what)
        => value <= 0 ? throw new DomainException($"{what} must be positive.") : value;

    public static void That(bool condition, string message)
    {
        if (!condition) throw new DomainException(message);
    }
}
