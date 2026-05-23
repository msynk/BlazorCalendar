using System.Globalization;

namespace BlazorFullCalendar;

public class BlazorFullCalendarState
{
    private List<BlazorFullCalendarEvent> _allEvents = [];
    private List<BlazorFullCalendarEvent> _filteredEvents = [];
    private List<BlazorFullCalendarResource> _resources = [];

    public DateTime SelectedDate { get; private set; } = DateTime.Today;
    public BlazorFullCalendarView View { get; private set; } = BlazorFullCalendarView.Month;
    public BlazorFullCalendarMode Mode { get; private set; } = BlazorFullCalendarMode.Event;
    public List<BlazorFullCalendarEventColor> SelectedColors { get; private set; } = [];

    /// <summary>When set, only events that include this attendee (by <see cref="BlazorFullCalendarHelpers.AttendeeFilterKey"/>) are shown.</summary>
    public string? SelectedAttendeeKey { get; private set; }
    public bool Use24HourFormat { get; private set; } = true;
    public BlazorFullCalendarBadgeVariant BadgeVariant { get; private set; } = BlazorFullCalendarBadgeVariant.Colored;
    public int StartOfDayHour { get; private set; } = 8;
    public BlazorFullCalendarAgendaGroupBy AgendaModeGroupBy { get; private set; } = BlazorFullCalendarAgendaGroupBy.Date;
    public bool IsDarkMode { get; private set; }

    /// <summary>Incremented when <see cref="GoToToday"/> is invoked in agenda view so the list can scroll to today.</summary>
    public ulong AgendaScrollToTodayNonce { get; private set; }

    public CultureInfo Culture { get; private set; } = CultureInfo.CurrentUICulture;
    public bool IsRtl => Culture.TextInfo.IsRightToLeft;

    // Drag state
    public BlazorFullCalendarEvent? DraggedEvent { get; set; }
    public bool IsDragging => DraggedEvent != null;

    public IReadOnlyList<BlazorFullCalendarEvent> Events => _filteredEvents;
    public IReadOnlyList<BlazorFullCalendarEvent> AllEvents => _allEvents;
    public IReadOnlyList<BlazorFullCalendarResource> Resources => _resources;

    public event Action? OnStateChanged;
    public event Action<BlazorFullCalendarDateChangeEventArgs>? OnDateRangeChanged;

    public void Initialize(List<BlazorFullCalendarEvent> events, CultureInfo? culture = null)
    {
        _allEvents = [.. events];
        if (culture != null)
            Culture = culture;
        UpdateUI();
    }

    public void SetCulture(CultureInfo culture)
    {
        Culture = culture;
        UpdateUI();
    }

    public void SetSelectedDate(DateTime date)
    {
        SelectedDate = date;
        UpdateUI();
        NotifyDateRangeChanged();
    }

    public void SetView(BlazorFullCalendarView view)
    {
        View = ClampViewForMode(view, Mode);
        UpdateUI();
        NotifyDateRangeChanged();
    }

    /// <summary>
    /// Switches between Event and Timeline modes. When entering Timeline mode the active view
    /// is clamped to Day / Week / Month (Year and Agenda are not supported in timeline mode).
    /// </summary>
    public void SetMode(BlazorFullCalendarMode mode)
    {
        if (Mode == mode)
            return;

        Mode = mode;
        var clamped = ClampViewForMode(View, mode);
        if (clamped != View)
            View = clamped;

        UpdateUI();
        NotifyDateRangeChanged();
    }

    private static BlazorFullCalendarView ClampViewForMode(BlazorFullCalendarView view, BlazorFullCalendarMode mode)
    {
        if (mode != BlazorFullCalendarMode.Timeline)
            return view;

        return view switch
        {
            BlazorFullCalendarView.Day or BlazorFullCalendarView.Week or BlazorFullCalendarView.Month => view,
            _ => BlazorFullCalendarView.Week
        };
    }

    public void SetUse24HourFormat(bool value)
    {
        if (Use24HourFormat == value)
            return;
        Use24HourFormat = value;
        NotifyStateChanged();
    }

    public void ToggleTimeFormat()
    {
        Use24HourFormat = !Use24HourFormat;
        NotifyStateChanged();
    }

    public void SetBadgeVariant(BlazorFullCalendarBadgeVariant variant)
    {
        if (BadgeVariant == variant)
            return;
        BadgeVariant = variant;
        NotifyStateChanged();
    }

    public void SetStartOfDayHour(int hour)
    {
        if (hour < 0 || hour > 16 || StartOfDayHour == hour)
            return;
        StartOfDayHour = hour;
        NotifyStateChanged();
    }

    public void SetAgendaModeGroupBy(BlazorFullCalendarAgendaGroupBy groupBy)
    {
        if (AgendaModeGroupBy == groupBy)
            return;
        AgendaModeGroupBy = groupBy;
        NotifyStateChanged();
    }

    public void SetDarkMode(bool value)
    {
        if (IsDarkMode == value)
            return;
        IsDarkMode = value;
        NotifyStateChanged();
    }

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        NotifyStateChanged();
    }

    public void NavigatePrevious()
    {
        SelectedDate = BlazorFullCalendarHelpers.NavigateDate(SelectedDate, View, false, Culture);
        UpdateUI();
        NotifyDateRangeChanged();
    }

    public void NavigateNext()
    {
        SelectedDate = BlazorFullCalendarHelpers.NavigateDate(SelectedDate, View, true, Culture);
        UpdateUI();
        NotifyDateRangeChanged();
    }

    public void GoToToday()
    {
        SelectedDate = DateTime.Today;
        if (View == BlazorFullCalendarView.Agenda)
            AgendaScrollToTodayNonce++;
        UpdateUI();
        NotifyDateRangeChanged();
    }

    /// <summary>
    /// Replaces the internal event list with the supplied collection when the contents differ.
    /// Safe to call from <c>OnParametersSet</c> — it short-circuits when the list hasn't changed,
    /// preventing infinite re-render loops.
    /// </summary>
    public void SyncEvents(List<BlazorFullCalendarEvent> events)
    {
        if (EventsMatch(events))
            return;

        _allEvents = [.. events];
        ApplyFilters();
        NotifyStateChanged();
    }

    /// <summary>
    /// Replaces the resource list shown by the resource timeline view. Safe to call from
    /// <c>OnParametersSet</c> — it short-circuits when the supplied list matches the current one.
    /// </summary>
    public void SyncResources(IReadOnlyList<BlazorFullCalendarResource>? resources)
    {
        var next = resources is null ? new List<BlazorFullCalendarResource>() : [.. resources];
        if (ResourcesMatch(next))
            return;

        _resources = next;
        NotifyStateChanged();
    }

    private bool ResourcesMatch(List<BlazorFullCalendarResource> resources)
    {
        if (_resources.Count != resources.Count)
            return false;

        for (var i = 0; i < _resources.Count; i++)
        {
            if (!ReferenceEquals(_resources[i], resources[i]))
                return false;
        }

        return true;
    }

    private bool EventsMatch(List<BlazorFullCalendarEvent> events)
    {
        if (_allEvents.Count != events.Count)
            return false;

        for (var i = 0; i < _allEvents.Count; i++)
        {
            if (!ReferenceEquals(_allEvents[i], events[i]))
                return false;
        }

        return true;
    }

    public void AddEvent(BlazorFullCalendarEvent ev)
    {
        _allEvents.Add(ev);
        UpdateUI();
    }

    public void UpdateEvent(BlazorFullCalendarEvent ev)
    {
        var idx = _allEvents.FindIndex(e => e.Id == ev.Id);
        if (idx >= 0) _allEvents[idx] = ev;
        UpdateUI();
    }

    public void RemoveEvent(string eventId)
    {
        _allEvents.RemoveAll(e => e.Id == eventId);
        UpdateUI();
    }

    public void FilterByColor(BlazorFullCalendarEventColor color)
    {
        if (SelectedColors.Contains(color))
            SelectedColors.Remove(color);
        else
            SelectedColors.Add(color);
        UpdateUI();
    }

    public void SetColorFilter(BlazorFullCalendarEventColor? color)
    {
        SelectedColors.Clear();
        if (color.HasValue)
            SelectedColors.Add(color.Value);

        UpdateUI();
    }

    public void SetAttendeeFilter(string? attendeeKey)
    {
        SelectedAttendeeKey = string.IsNullOrWhiteSpace(attendeeKey) ? null : attendeeKey.Trim();
        UpdateUI();
    }

    public void UpdateUI()
    {
        ApplyFilters();
        NotifyStateChanged();
    }

    /// <summary>Distinct attendees on events visible in the current view/date range.</summary>
    public IReadOnlyList<(string Key, string DisplayName)> GetAttendeesInCurrentView(string unnamedAttendeeText = "(Unnamed)")
    {
        var viewEvents = BlazorFullCalendarHelpers.GetEventsForView(_allEvents.ToList(), View, SelectedDate, Culture);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var ev in viewEvents)
        {
            foreach (var a in ev.Attendees)
            {
                var key = BlazorFullCalendarHelpers.AttendeeFilterKey(a);
                if (key.Length == 0)
                    continue;
                if (map.ContainsKey(key))
                    continue;
                var label = string.IsNullOrWhiteSpace(a.FullName)
                    ? (string.IsNullOrWhiteSpace(a.Id) ? unnamedAttendeeText : a.Id.Trim())
                    : a.FullName.Trim();
                map[key] = label;
            }
        }

        return map
            .OrderBy(kv => kv.Value, StringComparer.CurrentCultureIgnoreCase)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
    }

    public void ClearFilter()
    {
        SelectedColors.Clear();
        SelectedAttendeeKey = null;
        _filteredEvents = [.. _allEvents];
        NotifyStateChanged();
    }

    private void ApplyFilters()
    {
        PruneInvalidAttendeeFilter();

        var result = _allEvents.AsEnumerable();

        if (SelectedColors.Count > 0)
            result = result.Where(e => SelectedColors.Contains(e.Color));

        if (SelectedAttendeeKey is not null)
            result = result.Where(e => e.Attendees.Any(a => BlazorFullCalendarHelpers.AttendeeFilterKey(a) == SelectedAttendeeKey));

        _filteredEvents = result.ToList();
    }

    private void PruneInvalidAttendeeFilter()
    {
        if (SelectedAttendeeKey is null)
            return;

        var validKeys = BlazorFullCalendarHelpers
            .GetEventsForView(_allEvents.ToList(), View, SelectedDate, Culture)
            .SelectMany(e => e.Attendees)
            .Select(BlazorFullCalendarHelpers.AttendeeFilterKey)
            .Where(k => k.Length > 0)
            .ToHashSet(StringComparer.Ordinal);

        if (!validKeys.Contains(SelectedAttendeeKey))
            SelectedAttendeeKey = null;
    }

    // Drag-and-drop helpers
    public void StartDrag(BlazorFullCalendarEvent ev)
    {
        DraggedEvent = ev;
        NotifyStateChanged();
    }

    public void EndDrag()
    {
        if (DraggedEvent == null)
            return;

        DraggedEvent = null;
        NotifyStateChanged();
    }

    public void HandleDrop(DateTime targetDate, int? hour = null, int? minute = null)
        => HandleDrop(targetDate, hour, minute, resourceId: null, applyResource: false);

    /// <summary>
    /// Drops the currently dragged event onto a date/time and optionally re-assigns its
    /// <see cref="BlazorFullCalendarEvent.Resource"/>. When <paramref name="applyResource"/> is
    /// <c>false</c> the event keeps its existing resource. Pass <paramref name="resourceId"/> as
    /// <c>null</c> together with <paramref name="applyResource"/> = <c>true</c> to clear the
    /// resource (drop on the unassigned row).
    /// </summary>
    public void HandleDrop(DateTime targetDate, int? hour, int? minute, string? resourceId, bool applyResource)
    {
        if (DraggedEvent == null) return;

        var originalStart = DraggedEvent.StartDate;
        var originalResource = DraggedEvent.Resource;
        var duration = DraggedEvent.Duration;

        var newStart = targetDate.Date;
        if (hour.HasValue)
            newStart = newStart.AddHours(hour.Value).AddMinutes(minute ?? 0);
        else
            newStart = newStart.AddHours(originalStart.Hour).AddMinutes(originalStart.Minute);

        var newResource = applyResource ? resourceId : originalResource;

        var resourceChanged = applyResource && !string.Equals(originalResource ?? "", newResource ?? "", StringComparison.Ordinal);

        if (newStart == originalStart && !resourceChanged)
        {
            EndDrag();
            return;
        }

        var updated = new BlazorFullCalendarEvent
        {
            Id = DraggedEvent.Id,
            Title = DraggedEvent.Title,
            Description = DraggedEvent.Description,
            StartDate = newStart,
            EndDate = newStart + duration,
            Color = DraggedEvent.Color,
            Resource = newResource,
            Data = DraggedEvent.Data,
            Attendees = [.. DraggedEvent.Attendees]
        };

        UpdateEvent(updated);
        EndDrag();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();

    private void NotifyDateRangeChanged()
    {
        if (OnDateRangeChanged is null) return;
        var (start, end) = BlazorFullCalendarHelpers.GetDateRange(View, SelectedDate, Culture);
        OnDateRangeChanged.Invoke(new BlazorFullCalendarDateChangeEventArgs
        {
            Start = start,
            End = end,
            View = View
        });
    }
}

