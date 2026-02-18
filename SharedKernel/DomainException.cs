namespace DddBlazorDemo.SharedKernel;

public sealed class DomainException(string message) : Exception(message)
{
}
