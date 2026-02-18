using System.Text.Json;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class DomainEventSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Serialize(IDomainEvent evt) => JsonSerializer.Serialize(evt, evt.GetType(), Options);

    public IDomainEvent Deserialize(string typeName, string payloadJson, DomainEventTypeRegistry registry)
    {
        var type = registry.Resolve(typeName);
        var obj = JsonSerializer.Deserialize(payloadJson, type, Options)
                  ?? throw new InvalidOperationException($"Failed to deserialize event '{typeName}'.");
        return (IDomainEvent)obj;
    }
}
