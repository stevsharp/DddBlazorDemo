using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record Money
{
    public const int MaxDecimalPlaces = 2;

    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        Guard.That(DecimalPlaces(amount) <= MaxDecimalPlaces, $"Amount may have at most {MaxDecimalPlaces} decimal places.");

        Amount = amount;
        Currency = Guard.NotNull(currency, "Currency");
    }

    public static Money Zero(Currency currency) => new(0m, currency);

    public Money Add(Money other)
    {
        Guard.NotNull(other, "Amount");
        Guard.That(Currency == other.Currency, "Currency mismatch.");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int qty)
    {
        Guard.That(qty >= 0, "Quantity cannot be negative.");

        return new Money(Amount * qty, Currency);
    }

    private static int DecimalPlaces(decimal value)
        => (decimal.GetBits(value)[3] >> 16) & 0xFF;
}
