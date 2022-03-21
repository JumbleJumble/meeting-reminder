using System;
using System.Diagnostics;
using MeetingReminder.Services;

namespace MeetingReminder;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly IScheduleManager scheduleManager;

    public MainWindow(
        IScheduleManager scheduleManager,
        IAlertsManager alertsManager)
    {
        this.scheduleManager = scheduleManager;
        InitializeComponent();
        DateLabel.Text = DateTime.Today.ToString("d MMMM yyyy");
        scheduleManager.ScheduleRefreshed += ScheduleRefreshed;
        alertsManager.AlertRaised += AlertRaised;
    }

    private void ScheduleRefreshed()
    {
        EventsList.ItemsSource = scheduleManager.UpcomingEvents;
    }

    private void AlertRaised(Alert alert)
    {
        Debug.WriteLine(alert);
    }
}
