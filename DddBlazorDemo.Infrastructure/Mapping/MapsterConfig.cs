using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.Documents.Domain;
using Mapster;

namespace DddBlazorDemo.Infrastructure.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Invoice, InvoiceListItemDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.Number, s => s.Number.Value)
            .Map(d => d.ProviderEan, s => s.ProviderEan.Value)
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.TotalAmount, s => s.Total().Amount)
            .Map(d => d.CurrencyCode, s => s.Currency.Code);

        config.NewConfig<Document, DocumentListItemDto>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.InvoiceId, s => s.InvoiceId)
            .Map(d => d.FileName, s => s.FileName)
            .Map(d => d.IsPdfUpToDate, s => s.IsPdfUpToDate)
            .Map(d => d.WorkflowStatus, s => s.WorkflowStatus.ToString())
            .Map(d => d.CreatedAtUtc, s => s.CreatedAtUtc);
    }
}
