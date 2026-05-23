namespace BlazorFullCalendar;

/// <summary>
/// A schedulable resource shown as a row in the resource timeline view (for example,
/// a meeting room, a person, a piece of equipment).
/// Events are linked to a resource through <see cref="BlazorFullCalendarEvent.Resource"/>
/// matching <see cref="Id"/>.
/// </summary>
public sealed class BlazorFullCalendarResource
{
    /// <summary>
    /// Stable identifier matched against <see cref="BlazorFullCalendarEvent.Resource"/>.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the resource (for example "Bay Wing", "Alice Johnson", "Meeting Room 3B").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional group label used to visually group resource rows (for example building, department).
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Optional consumer-defined payload available to templates and click handlers.
    /// </summary>
    public object? Data { get; set; }
}
