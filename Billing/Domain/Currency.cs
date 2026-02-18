using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record Currency
{
    public string Code { get; }

    public Currency(string code)
    {
        code = (code ?? string.Empty).Trim().ToUpperInvariant();

        if (code.Length != 3)
            throw new DomainException("Currency code must be a 3-letter ISO code.");

        for (var i = 0; i < code.Length; i++)
        {
            if (!char.IsLetter(code[i]))
                throw new DomainException("Currency code must contain only letters.");
        }

        Code = code;
    }

    public override string ToString() => Code;

    public static Currency EUR => new("EUR");
}
