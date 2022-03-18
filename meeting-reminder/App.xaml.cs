using Autofac;
using MeetingReminder.Services;
using System.Windows;

namespace MeetingReminder;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IContainer container;

    public App()
    {
        container = ConfigureAutofac();
    }

    private IContainer ConfigureAutofac()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<MainWindow>().AsSelf();
        builder.RegisterType<CredentialProvider>().AsImplementedInterfaces();
        builder.RegisterType<CalendarServiceProvider>().AsImplementedInterfaces();
        builder.RegisterType<EventsListProvider>().AsImplementedInterfaces();
        builder.RegisterType<EventsCache>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<IntervalTimer>().AsImplementedInterfaces();
        return builder.Build();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = container.Resolve<MainWindow>();
        mainWindow.Show();
    }
}
