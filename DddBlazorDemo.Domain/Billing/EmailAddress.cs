using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record EmailAddress
{
    public const int MaxLength = 254;

    public string Value { get; }

    public EmailAddress(string value)
    {
        value = Guard.NotBlank(value, "Email").ToLowerInvariant();
        Guard.MaxLength(value, MaxLength, "Email");

        var at = value.IndexOf('@');
        Guard.That(at > 0 && at == value.LastIndexOf('@') && at < value.Length - 1,
            "Email must look like name@domain.");
        Guard.That(value[(at + 1)..].Contains('.') && !value.Any(char.IsWhiteSpace),
            "Email must look like name@domain.");

        Value = value;
    }

    public override string ToString() => Value;
}
