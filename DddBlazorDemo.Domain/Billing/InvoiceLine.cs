using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed class InvoiceLine
{
    public const int MaxDescriptionLength = 200;

    private InvoiceLine() { }

    public Guid Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = Money.Zero(Currency.EUR);

    public InvoiceLine(Guid id, string description, Money amount)
    {
        Id = Guard.NotEmpty(id, "Line id");
        Description = Guard.MaxLength(Guard.NotBlank(description, "Line description"), MaxDescriptionLength, "Line description");

        Guard.NotNull(amount, "Line amount");
        Guard.Positive(amount.Amount, "Line amount");
        Amount = amount;
    }
}
