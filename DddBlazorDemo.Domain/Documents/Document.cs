using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Documents.Domain;

/// <summary>
/// Workflow: Created -> PdfGenerated -> ReadyToSend. Transitions only move forward.
/// </summary>
public sealed class Document : Entity, IAggregateRoot
{
    public const int MaxFileNameLength = 128;

    private Document() { }

    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public bool IsPdfUpToDate { get; private set; }
    public DocumentWorkflowStatus WorkflowStatus { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Document CreateFromValidatedInvoice(Guid invoiceId, string invoiceNumber)
    {
        Guard.NotEmpty(invoiceId, "Invoice id");
        invoiceNumber = Guard.NotBlank(invoiceNumber, "Invoice number");

        return new Document
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoiceId,
            FileName = Guard.MaxLength($"Invoice_{ToFileNameSafe(invoiceNumber)}.pdf", MaxFileNameLength, "File name"),
            IsPdfUpToDate = false,
            WorkflowStatus = DocumentWorkflowStatus.Created,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>Invoice numbers may contain '/', which is not valid in a file name.</summary>
    private static string ToFileNameSafe(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Create(value.Length, value, (span, src) =>
        {
            for (var i = 0; i < src.Length; i++)
                span[i] = Array.IndexOf(invalid, src[i]) >= 0 ? '-' : src[i];
        });
    }

    public void MarkPdfGenerated()
    {
        Guard.That(WorkflowStatus != DocumentWorkflowStatus.ReadyToSend, "Document is already ready to send.");

        IsPdfUpToDate = true;
        WorkflowStatus = DocumentWorkflowStatus.PdfGenerated;
    }

    public void MarkReadyToSend()
    {
        Guard.That(IsPdfUpToDate, "PDF must be generated first.");
        Guard.That(WorkflowStatus != DocumentWorkflowStatus.ReadyToSend, "Document is already ready to send.");

        WorkflowStatus = DocumentWorkflowStatus.ReadyToSend;
    }
}
