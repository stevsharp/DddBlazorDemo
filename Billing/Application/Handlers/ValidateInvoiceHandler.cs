using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class ValidateInvoiceHandler(IDbContextFactory<AppDbContext> dbFactory) : 
    ICommandHandler<ValidateInvoiceCommand, Unit>
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory = dbFactory;

    public async Task<Unit> Handle(ValidateInvoiceCommand command, CancellationToken ct)
    {
        var invoice = await _dbFactory.CreateDbContext().Invoices.FirstAsync(x => x.Id == command.InvoiceId, ct);

        invoice.Validate();

        return Unit.Value;
    }
}
