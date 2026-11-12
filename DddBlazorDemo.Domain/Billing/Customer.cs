
using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Domain.Billing;

public sealed class Customer : Entity, IAggregateRoot
{
    private Customer() { }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public EanNumber Gln { get; private set; } = new("00000000");
    public bool IsActive { get; private set; } = true;

    public static Customer Create(string name, EanNumber gln) => new()
    {
        Id = Guid.NewGuid(),
        Name = Guard.NotBlank(name, "Customer name"),
        Gln = Guard.NotNull(gln, "Customer GLN")
    };

    public void Rename(string newName) => Name = Guard.NotBlank(newName, "Customer name");

    public BillTo ToBillTo() => new(Name, Gln);
}

public sealed record BillTo
{
    public const int MaxNameLength = 200;
    public string Name { get; }
    public EanNumber Gln { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="gln"></param>
    public BillTo(string name, EanNumber gln)
    {
        Name = Guard.NotBlank(name, "Customer name");
        Guard.MaxLength(Name, MaxNameLength, "Customer name");
        Gln = Guard.NotNull(gln, "Customer GLN");
    }
}
