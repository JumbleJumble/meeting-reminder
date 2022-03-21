using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeetingReminder.Services;

public class Alert
{
    public Alert(Appointment eventInfo, int minutesBefore)
    {
        this.eventInfo = eventInfo;
        this.minutesBefore = minutesBefore;
    }

    public Alert()
    {
    }

    public Appointment? eventInfo { get; set; }
    public int minutesBefore { get; set; }
    public ITimedCall? timedCall { get; set; }

    public override string ToString()
    {
        return $"{eventInfo?.Title} {eventInfo?.Start} - {minutesBefore} mins before";
    }
}

public interface IAlertsManager
{
    Action<Alert> AlertRaised { get; set; }
}

public class AlertsManager
    : IAlertsManager
{
    private readonly IScheduleManager scheduleManager;
    private readonly IIntervalTimer intervalTimer;
    private readonly IConfigProvider configProvider;
    private readonly Dictionary<EventInfo, List<Alert>> alerts = new ();

    public Action<Alert> AlertRaised { get; set; } = _ => { };

    public AlertsManager(
        IScheduleManager scheduleManager,
        IIntervalTimer intervalTimer,
        IConfigProvider configProvider)

    {
        this.scheduleManager = scheduleManager;
        this.intervalTimer = intervalTimer;
        this.configProvider = configProvider;
        scheduleManager.ScheduleRefreshed += ScheduleRefreshed;
    }

    private void ScheduleRefreshed()
    {
        var events = scheduleManager.UpcomingEvents.ToList();
        AddNewAlerts(events);
        RemoveDeletedEvents(events);
    }

    private void AddNewAlerts(IEnumerable<EventInfo> events)
    {
        foreach (var eventInfo in events)
        {
            if (alerts.ContainsKey(eventInfo))
            {
                continue;
            }

            var alertsForThisEvent = new List<Alert>();
            foreach (var minutesBefore in configProvider.Config.AlertMinutesBefore)
            {
                // don't do alerts for all day events
                if (eventInfo is not Appointment appointment)
                {
                    continue;
                }

                // don't schedule alerts that would be in the past
                var alertTime = appointment.Start - TimeSpan.FromMinutes(minutesBefore);
                if (alertTime.ToUniversalTime() < DateTimeOffset.Now.ToUniversalTime())
                {
                    continue;
                }

                var alert = new Alert(appointment, minutesBefore);
                var timedCall = intervalTimer.Call(() => AlertRaised(alert)).At(alertTime);
                alert.timedCall = timedCall;
                alertsForThisEvent.Add(alert);
                Debug.WriteLine($"Created alert: {alert}");
            }

            alerts[eventInfo] = alertsForThisEvent;
        }
    }

    private void RemoveDeletedEvents(ICollection<EventInfo> events)
    {
        var toRemove = new List<EventInfo>();

        foreach (var eventInfo in alerts.Keys)
        {
            if (events.Contains(eventInfo))
            {
                continue;
            }

            foreach (var alert in alerts[eventInfo])
            {
                Debug.WriteLine($"Deleting alert {alert}");
                alert.timedCall?.Stop();
            }

            toRemove.Add(eventInfo);
        }

        foreach (var eventToRemove in toRemove)
        {
            Debug.WriteLine($"Deleting event {eventToRemove}");
            alerts.Remove(eventToRemove);
        }
    }
}
