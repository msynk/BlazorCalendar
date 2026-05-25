namespace BlazorFullCalendar;

public class BlazorFullCalendarEvent
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    /// <summary>
    /// Identifier of the color (matches a <see cref="BlazorFullCalendarColorOption.Id"/> from the
    /// calendar's configured palette). Defaults to <see cref="BlazorFullCalendarColorScheme.FallbackColorId"/>
    /// so that out-of-the-box rendering keeps working with the built-in palette.
    /// </summary>
    public string Color { get; set; } = BlazorFullCalendarColorScheme.FallbackColorId;
    public List<BlazorFullCalendarAttendee> Attendees { get; set; } = [];

    /// <summary>
    /// Optional resource identifier linking this event to a <see cref="BlazorFullCalendarResource"/>
    /// (for example a meeting room name or a machine id). Used by the resource timeline view to
    /// place the event on the matching resource row. <c>null</c> or empty means the event is unassigned.
    /// </summary>
    public string? Resource { get; set; }

    public bool IsSingleDay => StartDate.Date == EndDate.Date;
    public bool IsMultiDay => !IsSingleDay;
    public TimeSpan Duration => EndDate - StartDate;

    public object? Data { get; set; }
}

