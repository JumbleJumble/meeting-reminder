using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingReminder.Services;

public abstract record EventInfo(string Title, string Description);
public record AllDayEvent(string Title, string Description) : EventInfo(Title, Description);

public record Appointment(
        string Title,
        string Description,
        DateTimeOffset Start,
        DateTimeOffset End)
    : EventInfo(Title, Description);

public record MeetAppointment(
        string Title,
        string Description,
        DateTimeOffset Start,
        DateTimeOffset End,
        string MeetUrl)
    : Appointment(Title, Description, Start, End);

public interface IEventsCache
{
    ICollection<EventInfo> Events { get; }

    Task RefreshAsync();
}

public class EventsCache : IEventsCache
{
    private readonly IEventsListProvider eventsListProvider;

    public EventsCache(IEventsListProvider eventsListProvider)
    {
        this.eventsListProvider = eventsListProvider;
    }

    private ICollection<EventInfo> events = new List<EventInfo>();

    public ICollection<EventInfo> Events => events;

    private void SetEvents(ICollection<EventInfo> value)
    {
        events = value;
    }

    public async Task RefreshAsync()
    {
        Events.Clear();
        var eventsList = await eventsListProvider.GetEventsAsync();
        foreach (var item in eventsList.Items)
        {
            if (item is null)
            {
                continue;
            }

            EventInfo? evt = item switch
            {
                { Start.Date: not null, End.Date: not null } => new AllDayEvent(item.Summary, item.Description),

                { ConferenceData: null, Start.DateTime: not null, End.DateTime: not null } => new Appointment(
                    item.Summary,
                    item.Description,
                    item.Start.DateTime.Value.ToUniversalTime(),
                    item.End.DateTime.Value.ToUniversalTime()),

                { ConferenceData.EntryPoints.Count: > 0, Start.DateTime: not null, End.DateTime: not null } => new
                    MeetAppointment(
                        item.Summary,
                        item.Description,
                        item.Start.DateTime.Value.ToUniversalTime(),
                        item.End.DateTime.Value.ToUniversalTime(),
                        item.ConferenceData.EntryPoints[0].Uri),

                _ => null
            };

            if (evt is not null)
            {
                Events.Add(evt);
            }
        }
    }
}
