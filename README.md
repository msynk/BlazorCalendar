**Note**: moved to bit BlazorUI (https://blazorui.bitplatform.dev).

Check its source code here: https://github.com/bitfoundation/bitplatform/tree/develop/src/BlazorUI

---
 
 # BlazorFullCalendar

A feature-rich, interactive calendar component for Blazor applications. Built with pure Blazor and .NET — no JavaScript frameworks required.

## Features

- **6 View Modes**: Day, Week, Month, Year, Agenda, plus a top-level **Timeline** mode (resource × time)
- **Timeline mode**: Sits next to the default Events mode. Inside Timeline you keep day, week, and month views, but rows are resources (rooms, machines, people) and columns are time. Day/week use one-hour columns; month uses one-day columns. The resource column stays pinned to the start while the timeline scrolls horizontally; on first paint the timeline auto-scrolls to the configured start-of-day hour (and to today's column when today is in range). Drag events between rows to reassign their resource
- **Event Management**: Create, edit, and delete events with a polished dialog and form validation
- **Custom Add UI (`OnAddClick`)**: Suppress the built-in add dialog and receive a draft event so you can show your own creation experience
- **Custom Event Click (`OnEventClick`)**: Suppress the built-in event details dialog and handle event clicks yourself
- **Date Range Changes (`OnDateChange`)**: React when the visible range changes (prev/next/today navigation or switching views) with inclusive start/end dates and the active view
- **Per-View Event Templates**: Customize event card content with `DayEventTemplate`, `WeekEventTemplate`, `MonthEventTemplate`, and `TimelineEventTemplate`
- **Programmatic Options (`Options`)**: Drive initial preferences (dark mode, time format, badge variant, start hour, agenda grouping) from code
- **Culture-Aware Date-Time Picker**: Built-in dropdown date-time picker in add/edit dialogs (no browser-native `datetime-local`) with culture calendar rendering support (including `fa-IR`)
- **Drag & Drop**: Move events between time slots and dates with native HTML5 drag-and-drop
- **Resize**: Drag the top or bottom handle of any day/week event block to adjust its start or end time
- **Smart Overlap Layout**: When events conflict in day/week views their cards stack diagonally (each card offset to the right) instead of being squeezed into thin equal-width columns, so the title and time stay readable. Hovering or focusing a card lifts it above the rest. Non-conflicting events keep the full column width
- **Event Tooltips**: Every event card exposes a native tooltip with the title, time range, and description so users can distinguish overlapping or clipped cards on hover
- **Multi-User Support**: Filter events by attendee or color with avatar initials and color badges
- **Fully customizable colors**: Bring your own palette through `EventColorOptions` — each entry has its own `Id`, `Title` (the full name shown verbatim, like `"SkyBlue"`), and `Value` (any CSS color). The component derives badges, swatches, and bullets from that single value
- **External Filter UI (`HideFilters`)**: Hide the built-in color and attendee dropdowns and supply your own filter controls with pre-filtered events
- **Text Customization**: Override UI labels, button text, placeholders, aria labels, and validation messages with `BlazorFullCalendarTexts`
- **Customizable**: Dark mode, 12/24-hour format, dot vs colored badges, configurable start hour, agenda grouping, and hideable settings/filters
- **Live Timeline**: Real-time current-time indicator in day and week views with "Happening Now" sidebar
- **Themes**: Default and Fluent (WinUI-style) built-in themes; dark mode supported for both
- **Self-Loading Assets**: The component can inject its own CSS and JS automatically — no manual `<link>` or `<script>` tags required

## Installation

### 1. Install the NuGet package

```bash
dotnet add package BlazorFullCalendar
```

Or add a project reference if you are working from this repository.

### 2. Register the assembly

**Blazor Server / Interactive Server**

```csharp
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(BlazorFullCalendar.BlazorFullCalendarAssembly.Value);
```

**Blazor WebAssembly**

```csharp
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// The assembly is discovered automatically for WASM.
```

### 3. Add the namespace import

In your `_Imports.razor`:

```razor
@using BlazorFullCalendar
```

### 4. Asset loading

By default the component automatically injects its stylesheet and JavaScript into the page the first time it renders (`LoadAssets="true"`). **No extra tags are needed in your host page.**

If you prefer to control asset loading yourself — for example to set a specific load order, use a bundler, or serve the files from a CDN — set `LoadAssets="false"` and add the tags manually to your host page (`index.html` or `App.razor`):

```html
<link rel="stylesheet" href="_content/BlazorFullCalendar/css/blazor-fullcalendar.css" />
<script src="_content/BlazorFullCalendar/js/blazor-fullcalendar.js"></script>
```

> The Fluent theme overrides are bundled inside `blazor-fullcalendar.css` and activated automatically when `Theme="BlazorFullCalendarTheme.Fluent"` is set on the component — no second stylesheet is required.

---

## Usage

### Basic Example

```razor
@page "/calendar"

<BlazorFullCalendar Events="myEvents"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />

@code {
    private List<BlazorFullCalendarEvent> myEvents = new()
    {
        new() {
            Id = "1",
            Title = "Team Meeting",
            Description = "Weekly sync",
            StartDate = DateTime.Today.AddHours(10),
            EndDate = DateTime.Today.AddHours(11),
            Color = "blue"
        }
    };

    private Task HandleCalendarChange(BlazorFullCalendarChangeEventArgs args)
    {
        // Persist args.Event or synchronize with your backend/store.
        return Task.CompletedTask;
    }
}
```

### Fluent Theme Example

```razor
<BlazorFullCalendar Events="myEvents"
                    Theme="BlazorFullCalendarTheme.Fluent"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />
```

### Manual Asset Loading Example

```razor
<!-- In your host page when LoadAssets="false" -->
<link rel="stylesheet" href="_content/BlazorFullCalendar/css/blazor-fullcalendar.css" />
<script src="_content/BlazorFullCalendar/js/blazor-fullcalendar.js"></script>
```

```razor
<BlazorFullCalendar Events="myEvents"
                    LoadAssets="false"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />
```

### Resource Timeline Example (`Resources`)

The Timeline **mode** is a top-level layout that sits alongside the default **Events** mode. It appears in the header automatically when `Resources` is supplied. Inside Timeline mode you keep the day, week, and month sub-views — but rows are resources and columns are time. Day and week sub-views use one-hour columns; the month sub-view uses one-day columns. The grid scrolls horizontally when the time axis exceeds the visible width. Tag each event with `Resource = "<resource-id>"` to anchor it to a row; events without a matching id land in the auto-added "Unassigned" row. Dragging an event between rows fires `OnChange` with `Source = Drag`.

```razor
<BlazorFullCalendar Events="events"
                    Resources="rooms"
                    InitialMode="BlazorFullCalendarMode.Timeline"
                    OnChange="HandleChange"
                    @rendermode="InteractiveServer" />

@code {
    private readonly List<BlazorFullCalendarResource> rooms =
    [
        new() { Id = "room-bay",    Title = "HQ - Bay Wing",   Subtitle = "Headquarters" },
        new() { Id = "room-garden", Title = "The Garden",      Subtitle = "Headquarters" },
        new() { Id = "room-war",    Title = "War Room (B1)",   Subtitle = "Basement" },
    ];

    private readonly List<BlazorFullCalendarEvent> events =
    [
        new()
        {
            Id = "1",
            Title = "Design Review",
            StartDate = DateTime.Today.AddHours(10),
            EndDate = DateTime.Today.AddHours(11),
            Resource = "room-bay",
            Color = "purple"
        }
    ];

    private Task HandleChange(BlazorFullCalendarChangeEventArgs args) => Task.CompletedTask;
}
```

### Custom Add UI Example (`OnAddClick`)

When `OnAddClick` is assigned the built-in add dialog is suppressed. The callback receives a draft event with `StartDate`/`EndDate` pre-filled from the clicked slot. Show your own creation UI and add the event to `Events` after persisting.

```razor
<BlazorFullCalendar Events="myEvents"
                    OnAddClick="HandleAdd"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />

@code {
    private List<BlazorFullCalendarEvent> myEvents = new();

    private Task HandleAdd(BlazorFullCalendarEvent? ev)
    {
        if (ev is null) return Task.CompletedTask;

        // Open your own creation dialog with the pre-filled draft

        return Task.CompletedTask;
    }

    private Task HandleCalendarChange(BlazorFullCalendarChangeEventArgs args)
        => Task.CompletedTask;
}
```

### Custom Event Click Example (`OnEventClick`)

When `OnEventClick` is assigned the built-in event details dialog is suppressed when any event is clicked (in all views). The callback receives the clicked `BlazorFullCalendarEvent` so you can show your own details UI.

```razor
<BlazorFullCalendar Events="myEvents"
                    OnEventClick="HandleEventClick"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />

@code {
    private List<BlazorFullCalendarEvent> myEvents = new();

    private Task HandleEventClick(BlazorFullCalendarEvent ev)
    {
        // Show your own event details dialog / side panel / navigation
        return Task.CompletedTask;
    }

    private Task HandleCalendarChange(BlazorFullCalendarChangeEventArgs args)
        => Task.CompletedTask;
}
```

### Date Range Change Example (`OnDateChange`)

When `OnDateChange` is assigned, it is invoked after the user moves the calendar to a new visible range: **previous/next/today** in the header, or when **switching views** (day, week, month, year, agenda). The callback receives `BlazorFullCalendarDateChangeEventArgs` with **inclusive** `Start` and `End` dates for that range (for example one day in day view, one week in week view, one month in month/agenda views, one year in year view), plus the current `View`.

```razor
<BlazorFullCalendar Events="myEvents"
                    OnDateChange="HandleDateChange"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />

@code {
    private List<BlazorFullCalendarEvent> myEvents = new();

    private Task HandleDateChange(BlazorFullCalendarDateChangeEventArgs args)
    {
        // args.Start, args.End (inclusive), args.View
        // e.g. load events from an API for this range
        return Task.CompletedTask;
    }

    private Task HandleCalendarChange(BlazorFullCalendarChangeEventArgs args)
        => Task.CompletedTask;
}
```

### Hide Built-In Filters (`HideFilters`)

When `HideFilters` is `true`, the built-in color and attendee dropdown filters are removed from the calendar header. You can then provide your own external filter UI and pass pre-filtered events to the calendar.

```razor
<BlazorFullCalendar Events="filteredEvents"
                    HideFilters="true"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />

@code {
    private List<BlazorFullCalendarEvent> allEvents = new();
    private string searchText = "";

    private List<BlazorFullCalendarEvent> filteredEvents =>
        allEvents.Where(e =>
            e.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            e.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

    private Task HandleCalendarChange(BlazorFullCalendarChangeEventArgs args)
        => Task.CompletedTask;
}
```

### Hide Settings Button (`HideSettings`)

When `HideSettings` is `true`, the built-in settings gear button is hidden from the header. You can still drive all settings programmatically through the `Options` parameter.

```razor
<BlazorFullCalendar Events="myEvents"
                    HideSettings="true"
                    Options="calendarOptions"
                    OnChange="HandleCalendarChange"
                    @rendermode="InteractiveServer" />

@code {
    private BlazorFullCalendarOptions calendarOptions = new()
    {
        IsDarkMode = true,
        Use24HourFormat = false,
        StartOfDayHour = 6
    };
}
```

### Custom Event Templates

Replace the default card content inside any view by supplying a per-view `RenderFragment<BlazorFullCalendarEvent>`. Templates receive the event being rendered. Day/week/timeline templates render inside the time-grid block; the month template renders inside the day cell badge.

```razor
<BlazorFullCalendar Events="myEvents"
                    DayEventTemplate="EventCard"
                    WeekEventTemplate="EventCard"
                    TimelineEventTemplate="EventCard"
                    MonthEventTemplate="MonthBadge"
                    @rendermode="InteractiveServer" />

@code {
    private RenderFragment<BlazorFullCalendarEvent> EventCard => ev =>
        @<div style="display:flex;flex-direction:column;gap:2px;">
            <strong>@ev.Title</strong>
            @if (!string.IsNullOrWhiteSpace(ev.Description))
            {
                <span style="font-size:11px;opacity:.8;">@ev.Description</span>
            }
        </div>;

    private RenderFragment<BlazorFullCalendarEvent> MonthBadge => ev =>
        @<span>📌 @ev.Title</span>;
}
```

### Localization Notes

- The event add/edit dialog uses a custom dropdown date-time picker instead of native browser date inputs.
- Date cells, weekday headers, month names, and year/day values are rendered from the active `CultureInfo` calendar.
- This improves consistency for non-Gregorian cultures such as Persian (`fa-IR`) and other localized calendars.
- Dialog labels and validation text can be localized by supplying a customized `BlazorFullCalendarTexts` instance.
- Use `CultureName` (a plain string) instead of `Culture` (a `CultureInfo`) when using `@rendermode="InteractiveServer"`, because `CultureInfo` is not JSON-serializable by Blazor's parameter persistence.

### Text Customization Example

```razor
<BlazorFullCalendar Events="myEvents"
                    CultureName="fa-IR"
                    Texts="calendarTexts"
                    @rendermode="InteractiveServer" />

@code {
    private readonly BlazorFullCalendarTexts calendarTexts = new()
    {
        AddEventButton = "افزودن رویداد",
        AddEventDialogTitle = "افزودن رویداد جدید",
        StartDateTimeLabel = "تاریخ و زمان شروع",
        EndDateTimeLabel = "تاریخ و زمان پایان",
        CreateEventButton = "ایجاد رویداد"
    };
}
```

---

## Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Events` | `List<BlazorFullCalendarEvent>?` | `null` | List of calendar events to display |
| `Culture` | `CultureInfo?` | `CultureInfo.CurrentUICulture` | Sets calendar/date rendering and formatting. Do not use with `@rendermode="InteractiveServer"` — use `CultureName` instead |
| `CultureName` | `string?` | `null` | Culture name shortcut (e.g. `"fa-IR"`, `"ar-SA"`, `"fr-FR"`). Takes precedence over `Culture` when both are supplied |
| `Texts` | `BlazorFullCalendarTexts` | `new()` | Custom UI strings for labels, placeholders, action buttons, aria labels, and validation messages |
| `Theme` | `BlazorFullCalendarTheme` | `Default` | Visual theme — `Default` or `Fluent` (WinUI-style). Dark mode is supported for both |
| `EventColorOptions` | `IReadOnlyList<BlazorFullCalendarColorOption>?` | `null` | Ordered list of event colors shown in pickers, filters, agenda headers, badges, and bullets. Each entry has `Id` (matched against `BlazorFullCalendarEvent.Color`), `Title` (display label shown verbatim, e.g. `"SkyBlue"`), and `Value` (any CSS color). Defaults to `BlazorFullCalendarColorOption.Defaults` (the six original palettes) when `null` |
| `Resources` | `IReadOnlyList<BlazorFullCalendarResource>?` | `null` | Resources displayed as rows in Timeline mode. Each event's `Resource` property is matched against the resource `Id`. The Timeline mode tab is hidden when `null` or empty |
| `InitialMode` | `BlazorFullCalendarMode?` | `null` (Event) | Initial layout mode. `Event` shows day/week/month/year/agenda views. `Timeline` shows resources × time grid (day/week/month sub-views) and requires `Resources` to be non-empty |
| `OnChange` | `EventCallback<BlazorFullCalendarChangeEventArgs>` | — | Raised when a user adds, edits, or deletes an event (`Kind`: `Add`, `Edit`, `Delete`; `Source`: `Dialog`, `Drag`, `Resize`, `Delete`) |
| `OnAddClick` | `EventCallback<BlazorFullCalendarEvent?>` | — | When assigned, the built-in add dialog is suppressed. Receives a draft event with pre-filled dates from the clicked slot. Show your own creation UI and update `Events` after persisting |
| `OnEventClick` | `EventCallback<BlazorFullCalendarEvent>` | — | When assigned, the built-in event details dialog is suppressed when an event is clicked. Receives the clicked event so you can show your own details UI |
| `OnDateChange` | `EventCallback<BlazorFullCalendarDateChangeEventArgs>` | — | Raised when the visible date range changes after prev/next/today navigation or a view switch. Payload includes inclusive `Start`/`End` and the active `View` |
| `HideFilters` | `bool` | `false` | When `true`, hides the built-in color and attendee filter dropdowns. Consumers provide their own filter UI and pass pre-filtered events |
| `HideSettings` | `bool` | `false` | When `true`, hides the built-in settings gear button. Settings can still be driven programmatically through `Options` |
| `Options` | `BlazorFullCalendarOptions` | `new()` | Initial preferences — dark mode, 12/24-hour time format, badge variant, day start hour, and agenda grouping |
| `DayEventTemplate` | `RenderFragment<BlazorFullCalendarEvent>?` | `null` | Replaces the default event card content inside day-view time-grid blocks |
| `WeekEventTemplate` | `RenderFragment<BlazorFullCalendarEvent>?` | `null` | Replaces the default event card content inside week-view time-grid blocks |
| `MonthEventTemplate` | `RenderFragment<BlazorFullCalendarEvent>?` | `null` | Replaces the default event badge content inside month-view cells |
| `TimelineEventTemplate` | `RenderFragment<BlazorFullCalendarEvent>?` | `null` | Replaces the default event card content inside Timeline mode blocks |
| `LoadAssets` | `bool` | `true` | When `true` the component automatically injects its CSS and JS into the page on first render. Set to `false` to manage assets manually (see [Asset loading](#4-asset-loading)) |

---

## Models

### BlazorFullCalendarEvent

```csharp
public class BlazorFullCalendarEvent
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Color { get; set; }                    // matches a BlazorFullCalendarColorOption.Id
    public List<BlazorFullCalendarAttendee> Attendees { get; set; }

    /// <summary>
    /// Optional resource id (e.g. meeting room name) used by the resource timeline view
    /// to place the event on the matching <see cref="BlazorFullCalendarResource"/> row.
    /// Null/empty leaves the event unassigned.
    /// </summary>
    public string? Resource { get; set; }

    // Computed
    public bool IsSingleDay { get; }    // StartDate.Date == EndDate.Date
    public bool IsMultiDay { get; }     // !IsSingleDay
    public TimeSpan Duration { get; }   // EndDate - StartDate
}
```

### BlazorFullCalendarResource

```csharp
public sealed class BlazorFullCalendarResource
{
    public string Id { get; set; }         // matched against BlazorFullCalendarEvent.Resource
    public string Title { get; set; }      // display name shown on the timeline row
    public string? Subtitle { get; set; }  // optional subtitle shown below the title
    public object? Data { get; set; }
}
```

### BlazorFullCalendarAttendee

```csharp
public class BlazorFullCalendarAttendee
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Id { get; set; }

    // Computed
    public string FullName { get; }   // "FirstName LastName"
    public string Initials { get; }   // e.g. "AJ"
}
```

### BlazorFullCalendarColorOption

```csharp
public sealed class BlazorFullCalendarColorOption
{
    public string Id { get; set; }      // case-insensitive id matched against BlazorFullCalendarEvent.Color
    public string Title { get; set; }   // display label shown verbatim in pickers, filters, agenda headers
    public string Value { get; set; }   // any CSS color: "#3b82f6", "rgb(...)", "skyblue", etc.

    // Six default options (Ids: "blue", "green", "red", "yellow", "purple", "orange").
    public static IReadOnlyList<BlazorFullCalendarColorOption> Defaults { get; }
}
```

When `EventColorOptions` is omitted the component falls back to `BlazorFullCalendarColorOption.Defaults`. Supply your own list to fully control which colors appear, in what order, what label is shown, and the actual CSS color used in swatches and badges:

```csharp
private static readonly IReadOnlyList<BlazorFullCalendarColorOption> Palette =
[
    new() { Id = "sky",   Title = "SkyBlue",  Value = "skyblue" },
    new() { Id = "moss",  Title = "Moss",     Value = "#5b8a3a" },
    new() { Id = "rose",  Title = "Rose",     Value = "#e11d48" },
];

// Then on each event:
new BlazorFullCalendarEvent { /* ... */ Color = "sky" };
```

### BlazorFullCalendarChangeEventArgs

```csharp
public sealed class BlazorFullCalendarChangeEventArgs
{
    public required BlazorFullCalendarEvent Event { get; init; }   // new/updated state (or removed snapshot for Delete)
    public BlazorFullCalendarEvent? OldEvent { get; init; }        // previous state (Edit/Delete); null for Add
    public required BlazorFullCalendarChangeKind Kind { get; init; } // Add | Edit | Delete
    public BlazorFullCalendarChangeSource Source { get; init; }      // Dialog | Drag | Resize | Delete
}
```

### BlazorFullCalendarDateChangeEventArgs

Passed to `OnDateChange` when the user changes the visible range (header navigation or view tabs).

```csharp
public sealed class BlazorFullCalendarDateChangeEventArgs
{
    public required DateTime Start { get; init; }              // inclusive range start (date)
    public required DateTime End { get; init; }                // inclusive range end (date)
    public required BlazorFullCalendarView View { get; init; } // active view when the change occurred
}
```

---

## Views

- **Month View**: Grid layout with multi-day event support and "+N more" overflow
- **Week View**: 7-day view with hourly time slots, drag-and-drop, and event resize. Overlapping events are laid out as a diagonal stack — each conflicting card is offset to the right and hover/focus brings the obscured card to the front
- **Day View**: Single-day detailed view with timeline, sidebar mini-calendar, and "Happening Now" panel. Same diagonal-stack layout for overlapping events as Week view
- **Year View**: 12-month overview with per-day event bullet indicators
- **Agenda View**: Searchable list grouped by date or user
- **Timeline mode**: A separate top-level mode shown when `Resources` is supplied. Resources occupy the vertical axis and time the horizontal axis; the day, week, and month sub-views remain available inside Timeline. Day and week sub-views use one-hour columns; the month sub-view uses one-day columns to keep the time axis a sensible length. The resource column stays pinned to the leading edge while the time axis scrolls horizontally. On first paint the grid auto-scrolls to the start-of-day hour, and to today's day column when today falls inside the visible range. Drag events between rows to reassign their `Resource`

---

## Customization

The calendar includes built-in settings accessible via the gear icon in the header (hidden when `HideSettings="true"`):

- Toggle dark mode
- Switch between 12/24-hour time format
- Choose badge style (colored or dot)
- Set start hour for day/week views
- Configure agenda view grouping

All of these can also be set programmatically through the `Options` parameter. The built-in color and attendee filter dropdowns can be hidden with `HideFilters="true"` so you can provide your own external filter UI.

### CSS Customization

All CSS classes use the `bfc-` prefix (e.g. `bfc-root`, `bfc-header`, `bfc-btn`) and all CSS custom properties use `--bfc-` (e.g. `--bfc-primary`, `--bfc-bg`, `--bfc-border`). You can override any variable on `:root` or scope overrides to `.bfc-root`:

```css
.bfc-root {
    --bfc-primary: #8b5cf6;
    --bfc-radius: 12px;
}
```

Key CSS variables:

| Variable | Description |
|----------|-------------|
| `--bfc-bg` | Main background color |
| `--bfc-bg-secondary` | Secondary/subtle background |
| `--bfc-bg-hover` | Hover state background |
| `--bfc-border` | Border color |
| `--bfc-text` | Primary text color |
| `--bfc-text-secondary` | Secondary text color |
| `--bfc-text-muted` | Muted/disabled text color |
| `--bfc-primary` | Accent/brand color |
| `--bfc-primary-hover` | Accent hover state |
| `--bfc-primary-text` | Text on accent backgrounds |
| `--bfc-danger` | Destructive action color |
| `--bfc-radius` | Border radius for panels |
| `--bfc-radius-sm` | Border radius for small elements |
| `--bfc-shadow` | Default box shadow |
| `--bfc-shadow-lg` | Elevated box shadow |
| `--bfc-hour-height` | Height of one hour row in day/week views (default `96px`) |

Dark mode overrides are applied automatically via the `.bfc-dark` class.

---

## Static Asset Paths

When `LoadAssets="false"`, the files are served from the Razor Class Library's static web assets path:

| Asset | Path |
|-------|------|
| Stylesheet (base + Fluent theme) | `_content/BlazorFullCalendar/css/blazor-fullcalendar.css` |
| Fluent theme overrides only | `_content/BlazorFullCalendar/css/blazor-fullcalendar.fluent.css` |
| JavaScript interop | `_content/BlazorFullCalendar/js/blazor-fullcalendar.js` |

> `blazor-fullcalendar.css` already `@import`s `blazor-fullcalendar.fluent.css`, so you only need to reference the main stylesheet.

---

## Browser Support

- Modern browsers with CSS Grid and Flexbox support
- HTML5 Drag and Drop API support required for drag-and-drop

---

## Credits

Original concept inspired by [yassir-jeraidi/full-calendar](https://github.com/yassir-jeraidi/full-calendar)

## License

MIT, use it and be happy :)
