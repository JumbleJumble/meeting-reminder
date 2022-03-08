using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Threading.Tasks;

namespace MeetingReminder.Services
{
    public interface IEventsListProvider
    {
        Task<Events> GetEventsAsync();
    }

    internal class EventsListProvider : IEventsListProvider
    {
        private readonly ICalendarServiceProvider calendarServiceProvider;

        public EventsListProvider(ICalendarServiceProvider calendarServiceProvider)
        {
            this.calendarServiceProvider = calendarServiceProvider;
        }

        public async Task<Events> GetEventsAsync()
        {
            var service = await calendarServiceProvider.GetCalendarServiceAsync();

            var request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            return await request.ExecuteAsync();
        }
    }
}
