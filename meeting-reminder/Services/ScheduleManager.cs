using System;
using System.Collections.Generic;

namespace MeetingReminder.Services
{
    internal class ScheduleManager
    {
        private static TimeSpan WaitTime = TimeSpan.FromSeconds(10);
        private readonly IEventsCache eventsCache;
        private readonly IIntervalTimer intervalTimer;

        public ScheduleManager(
            IEventsCache eventsCache,
            IIntervalTimer intervalTimer)
        {
            this.eventsCache = eventsCache;
            this.intervalTimer = intervalTimer;
            UpdateSchedule();
        }

        private void UpdateSchedule()
        {
            eventsCache.RefreshAsync();
            intervalTimer.Call(UpdateSchedule).In(WaitTime);
        }

        public IEnumerable<EventInfo> UpcomingEvents
        {
            get
            {
                return new List<EventInfo>();
            }
        }
    }
}
