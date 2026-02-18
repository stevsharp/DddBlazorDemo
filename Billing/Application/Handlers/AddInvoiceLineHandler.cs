using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class AddInvoiceLineHandler(IDbContextFactory<AppDbContext> dbFactory) : ICommandHandler<AddInvoiceLineCommand, Unit>
{
    
    public async Task<Unit> Handle(AddInvoiceLineCommand command, CancellationToken ct)
    {
        var invoice = await dbFactory.CreateDbContext().Invoices.FirstAsync(x => x.Id == command.InvoiceId, ct);

        invoice.AddLine(command.Description, command.Amount);

        return Unit.Value;
    }
}
