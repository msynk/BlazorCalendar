namespace BlazorFullCalendar.Demo;

/// <summary>
/// Sample data for the Resource Timeline demo: a small set of meeting rooms / spaces
/// and a day's worth of events that book them.
/// </summary>
public static class DemoResourceData
{
    public static List<BlazorFullCalendarResource> CreateResources() =>
    [
        new() { Id = "room-bay",       Title = "HQ - Bay Wing",          Subtitle = "Headquarters" },
        new() { Id = "room-garden",    Title = "The Garden - Room 204",  Subtitle = "Headquarters" },
        new() { Id = "room-exec",      Title = "Executive Studio (14F)", Subtitle = "Headquarters" },
        new() { Id = "room-summit",    Title = "Summit Boardroom",       Subtitle = "Headquarters" },
        new() { Id = "room-atrium",    Title = "Atrium Lounge",          Subtitle = "Headquarters" },
        new() { Id = "room-deep-work", Title = "Deep Work Pods",         Subtitle = "Quiet Floor" },
        new() { Id = "room-library",   Title = "Library",                Subtitle = "Quiet Floor" },
        new() { Id = "room-phone",     Title = "Phone Booth A",          Subtitle = "Quiet Floor" },
        new() { Id = "room-war",       Title = "War Room (B1)",          Subtitle = "Basement" },
        new() { Id = "room-vault",     Title = "The Vault",              Subtitle = "Basement" },
        new() { Id = "room-workshop",  Title = "Workshop & Lab",         Subtitle = "Basement" },
        new() { Id = "room-customer",  Title = "Customer Lab",           Subtitle = "Ground Floor" },
        new() { Id = "room-cafe",      Title = "Cafe Corner",            Subtitle = "Ground Floor" },
        new() { Id = "room-rooftop",   Title = "Rooftop Terrace",        Subtitle = "Ground Floor" },
    ];

    public static List<BlazorFullCalendarEvent> CreateEvents()
    {
        var today = DateTime.Today;
        var id = 0;

        BlazorFullCalendarEvent E(
            string title,
            int dayOffset,
            int startHour, int startMin,
            int endDayOffset,
            int endHour, int endMin,
            BlazorFullCalendarEventColor color,
            string? resource,
            string? description = null)
        {
            return new BlazorFullCalendarEvent
            {
                Id = (++id).ToString(),
                Title = title,
                Description = description ?? string.Empty,
                StartDate = today.AddDays(dayOffset).AddHours(startHour).AddMinutes(startMin),
                EndDate = today.AddDays(endDayOffset).AddHours(endHour).AddMinutes(endMin),
                Color = color,
                Resource = resource
            };
        }

        return
        [
            // Today (day & week sub-views show these clearly)
            E("Team Standup",        0,  8, 30, 0,  9,  0, BlazorFullCalendarEventColor.Blue,   "room-bay",       "Daily sync."),
            E("Design Review",       0,  9,  0, 0, 10, 30, BlazorFullCalendarEventColor.Purple, "room-bay",       "Dashboard mockups v2."),
            E("Inbox zero",          0,  8,  0, 0,  8, 45, BlazorFullCalendarEventColor.Yellow, "room-deep-work", "Clear urgent email and Slack."),
            E("Code review",         0, 10,  0, 0, 11,  0, BlazorFullCalendarEventColor.Red,    "room-deep-work", "Auth module PRs."),
            E("Coffee chat",         0,  9, 30, 0, 10, 15, BlazorFullCalendarEventColor.Green,  "room-garden",    "Informal catch-up with design."),
            E("UX Testing",          0, 11,  0, 0, 12, 30, BlazorFullCalendarEventColor.Yellow, "room-garden",    "Checkout usability sessions."),
            E("Investor Pitch",      0, 13,  0, 0, 14,  0, BlazorFullCalendarEventColor.Orange, "room-exec",      "Series B deck walkthrough."),
            E("Board Meeting",       0, 14, 30, 0, 16, 30, BlazorFullCalendarEventColor.Purple, "room-exec",      "Monthly board session."),
            E("Architecture Review", 0, 10,  0, 0, 12,  0, BlazorFullCalendarEventColor.Red,    "room-war",       "Microservices migration plan."),
            E("Postmortem",          0, 13, 30, 0, 14, 30, BlazorFullCalendarEventColor.Red,    "room-war",       "Incident review - action items."),
            E("Customer Onboarding", 0, 10,  0, 0, 11, 30, BlazorFullCalendarEventColor.Green,  "room-customer",  "Platform walkthrough."),
            E("Product Demo",        0, 14,  0, 0, 15,  0, BlazorFullCalendarEventColor.Orange, "room-customer",  "Stakeholder feature walkthrough."),
            E("Lunch with Client",   0, 12,  0, 0, 13, 30, BlazorFullCalendarEventColor.Green,  "room-bay",       "Q3 roadmap discussion."),
            E("API pairing",         0, 15,  0, 0, 16, 30, BlazorFullCalendarEventColor.Red,    "room-bay",       "Implement rate limiting together."),
            E("Sprint Planning",     0, 14,  0, 0, 15, 30, BlazorFullCalendarEventColor.Orange, "room-garden",    "Next sprint goals and capacity."),
            E("Focus block",         0, 13,  0, 0, 15,  0, BlazorFullCalendarEventColor.Blue,   "room-deep-work", "Deep work - notifications off."),

            // Bookings on the additional spaces so every row has something visible.
            E("Quarterly Strategy",  0,  9,  0, 0, 10, 30, BlazorFullCalendarEventColor.Purple, "room-summit",    "Strategy roadmap with leads."),
            E("1:1 with Sam",        0, 14,  0, 0, 14, 30, BlazorFullCalendarEventColor.Blue,   "room-summit",    "Career growth chat."),
            E("Casual sync",         0, 11, 30, 0, 12,  0, BlazorFullCalendarEventColor.Green,  "room-atrium",    "Quick design pairing."),
            E("Reading time",        0,  9,  0, 0, 10,  0, BlazorFullCalendarEventColor.Yellow, "room-library",   "Catch up on RFCs."),
            E("Spec drafting",       0, 13,  0, 0, 15,  0, BlazorFullCalendarEventColor.Blue,   "room-library",   "Draft Q4 platform spec."),
            E("Recruiter call",      0, 10,  0, 0, 10, 30, BlazorFullCalendarEventColor.Orange, "room-phone",     "External candidate screen."),
            E("Vendor sync",         0, 15,  0, 0, 15, 45, BlazorFullCalendarEventColor.Yellow, "room-phone",     "Renewal walkthrough."),
            E("Backups review",      0, 11,  0, 0, 12,  0, BlazorFullCalendarEventColor.Red,    "room-vault",     "DR posture audit."),
            E("Hardware repair",     0, 13,  0, 0, 16,  0, BlazorFullCalendarEventColor.Orange, "room-workshop",  "Replace failing SSDs."),
            E("Coffee tasting",      0, 10, 30, 0, 11, 15, BlazorFullCalendarEventColor.Green,  "room-cafe",      "New roast trial with the team."),
            E("Happy hour",          0, 17,  0, 0, 18, 30, BlazorFullCalendarEventColor.Purple, "room-rooftop",   "End-of-week wind down."),

            // Other days this week (visible in timeline week view)
            E("All hands",           1, 10,  0, 1, 11,  0, BlazorFullCalendarEventColor.Blue,   "room-exec",      "Company-wide weekly."),
            E("UX workshop",         2, 13,  0, 2, 16,  0, BlazorFullCalendarEventColor.Purple, "room-garden",    "Mobile flows."),
            E("Vendor demo",         3,  9,  0, 3, 10,  0, BlazorFullCalendarEventColor.Yellow, "room-customer",  "New monitoring tooling."),

            // Multi-day spans (visible in timeline month view)
            E("Tech Conference",     2,  9,  0, 4, 17,  0, BlazorFullCalendarEventColor.Blue,   "room-exec",      "Keynotes, workshops, hallway track."),
            E("Hackathon",           7,  9,  0, 8, 17,  0, BlazorFullCalendarEventColor.Orange, "room-customer",  "48-hour build sprint."),
            E("Quarterly Maintenance", 14, 0, 0, 16, 23, 59, BlazorFullCalendarEventColor.Red,  "room-war",       "Scheduled downtime window."),

            // One unassigned event so the "Unassigned" row demonstrates how the calendar
            // surfaces events that have no resource yet.
            E("Networking event",    0, 17,  0, 0, 18, 30, BlazorFullCalendarEventColor.Blue,   null,             "Open invite - find a venue."),
        ];
    }
}
