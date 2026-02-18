using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Documents.Domain;

public sealed class Document : Entity, IAggregateRoot
{
    private Document() { }

    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public bool IsPdfUpToDate { get; private set; }
    public DocumentWorkflowStatus WorkflowStatus { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Document CreateFromValidatedInvoice(Guid invoiceId, string invoiceNumber)
    {
        return new Document
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoiceId,
            FileName = $"Invoice_{invoiceNumber}.pdf",
            IsPdfUpToDate = false,
            WorkflowStatus = DocumentWorkflowStatus.Created,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkPdfGenerated()
    {
        IsPdfUpToDate = true;
        WorkflowStatus = DocumentWorkflowStatus.PdfGenerated;
    }

    public void MarkReadyToSend()
    {
        if (!IsPdfUpToDate) 
            throw new DomainException("PDF must be generated first.");

        WorkflowStatus = DocumentWorkflowStatus.ReadyToSend;
    }
}
