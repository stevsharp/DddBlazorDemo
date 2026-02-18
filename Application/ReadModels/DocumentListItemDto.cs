namespace DddBlazorDemo.Application.ReadModels;

public sealed record DocumentListItemDto(
    Guid Id,
    Guid InvoiceId,
    string FileName,
    bool IsPdfUpToDate,
    string WorkflowStatus,
    DateTime CreatedAtUtc);
