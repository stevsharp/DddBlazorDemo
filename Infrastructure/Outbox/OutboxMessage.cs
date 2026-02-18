namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string Type { get; set; } = string.Empty;

    public string PayloadJson { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public int AttemptCount { get; set; }

    public DateTime? LastAttemptUtc { get; set; }

    public string? LastError { get; set; }

    public DateTime? LockedUntilUtc { get; set; }

    public string? LockOwner { get; set; }
}
