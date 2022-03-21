using System;
using System.Collections.Generic;

namespace MeetingReminder.Services
{
    public interface IScheduleManager
    {
        Action ScheduleRefreshed { get; set; }
        IEnumerable<EventInfo> UpcomingEvents { get; }
    }

    public class ScheduleManager
        : IScheduleManager
    {
        public Action ScheduleRefreshed { get; set; }

        private static TimeSpan WaitTime = TimeSpan.FromSeconds(10);
        private readonly IEventsCache eventsCache;
        private readonly IIntervalTimer intervalTimer;


        public ScheduleManager(
            IEventsCache eventsCache,
            IIntervalTimer intervalTimer)
        {
            this.eventsCache = eventsCache;
            this.intervalTimer = intervalTimer;
            ScheduleRefreshed = () => { };
            UpdateSchedule();
        }

        private async void UpdateSchedule()
        {
            await eventsCache.RefreshAsync();
            ScheduleRefreshed();
            intervalTimer.Call(UpdateSchedule).In(WaitTime);
        }

        public IEnumerable<EventInfo> UpcomingEvents
        {
            get
            {
                return eventsCache.Events;
            }
        }
    }
}
