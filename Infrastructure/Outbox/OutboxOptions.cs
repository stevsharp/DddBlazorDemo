namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int PollSeconds { get; set; } = 2;
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 10;
}
