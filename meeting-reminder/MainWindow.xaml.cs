using MeetingReminder.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MeetingReminder;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IEventsCache eventsCache;

    public MainWindow(IEventsCache eventsCache)
    {
        InitializeComponent();
        Loaded += GetEvents;
        this.eventsCache = eventsCache;
        DateLabel.Text = DateTime.Today.ToString("d MMMM yyyy");
    }

    private async void GetEvents(object sender, RoutedEventArgs e)
    {
        await eventsCache.RefreshAsync();
        EventsList.ItemsSource = eventsCache.Events;
    }
}
