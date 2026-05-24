using Microsoft.JSInterop;

namespace BlazorFullCalendar;

internal static class TimelineScrollInterop
{
    public static async ValueTask<bool> TryScrollToTargetAsync(
        IJSRuntime js,
        string scrollContainerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await js.InvokeAsync<bool>(
                "BlazorFullCalendar.scrollTimelineToTarget",
                cancellationToken,
                scrollContainerId);
        }
        catch (JSDisconnectedException)
        {
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (JSException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
