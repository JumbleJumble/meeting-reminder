using MeetingReminder.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MeetingReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEventsListProvider eventsListProvider;

        public MainWindow(IEventsListProvider eventsListProvider)
        {
            InitializeComponent();
            this.eventsListProvider = eventsListProvider;
            Loaded += GetEvents;
        }

        private async void GetEvents(object sender, RoutedEventArgs e)
        {
            var events = await eventsListProvider.GetEventsAsync();
            EventsList.ItemsSource = events.Items.Select(e => $"{e.Summary} - {e.Start.DateTime}: {e.ConferenceData?.EntryPoints?.FirstOrDefault()?.Uri ?? string.Empty}");
        }
    }
}
