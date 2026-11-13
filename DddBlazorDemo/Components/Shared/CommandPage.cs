using DddBlazorDemo.SharedKernel;
using Microsoft.AspNetCore.Components;

namespace DddBlazorDemo.Components.Shared;

/// <summary>
/// Base for pages that send commands: one busy flag, one status message, one Run() wrapper.
/// Domain errors are shown as-is; anything else is logged and shown as a generic message.
/// </summary>
public abstract class CommandPage : ComponentBase
{
    protected bool IsBusy { get; private set; }
    protected string? Message { get; private set; }
    protected bool IsError { get; private set; }

    protected abstract Task Reload();

    protected override async Task OnInitializedAsync() => await Reload();

    protected void ClearMessage() => Message = null;

    protected void ShowError(string message)
    {
        IsError = true;
        Message = message;
    }

    protected async Task Run(Func<Task> action, string successMessage)
    {
        if (IsBusy) return;

        IsBusy = true;
        Message = null;
        try
        {
            await action();
            await Reload();
            IsError = false;
            Message = successMessage;

            var shown = Message;
            _ = Task.Delay(3000).ContinueWith(_ => InvokeAsync(() =>
            {
                if (Message == shown)
                {
                    Message = null;
                    StateHasChanged();
                }
            }));
        }
        catch (DomainException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            ShowError("Something went wrong. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
