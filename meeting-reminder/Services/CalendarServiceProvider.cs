using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.Threading.Tasks;

namespace MeetingReminder.Services;

public interface ICalendarServiceProvider
{
    Task<CalendarService> GetCalendarServiceAsync();
}

public class CalendarServiceProvider : ICalendarServiceProvider
{
    const string ApplicationName = "Google Calendar API .NET Quickstart";
    private readonly ICredentialProvider credentialProvider;

    public CalendarServiceProvider(ICredentialProvider credentialProvider)
    {
        this.credentialProvider = credentialProvider;
    }

    public async Task<CalendarService> GetCalendarServiceAsync()
    {
        var credential = await credentialProvider.GetUserCredentialAsync();
        var service = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });

        return service;
    }
}
