using System;
using System.Threading.Tasks;

namespace MeetingReminder.Services;

internal interface IIntervalTimer
{
    ITimedCall Call(Action callback);
    ITimedCall CallAsync(Func<Task> callback);
}

internal class IntervalTimer
{
    public ITimedCall Call(Action callback)
    {
        return new TimedCall(callback);
    }
}

internal interface ITimedCall
{
    ITimedCall Every(TimeSpan interval);
    ITimedCall In(TimeSpan interval);
    void Stop();
}

internal class TimedCall : ITimedCall
{
    private Action callback;
    private bool stopped;

    public TimedCall(Action callback)
    {
        this.callback = callback;
    }

    public ITimedCall Every(TimeSpan interval)
    {
        Impl();
        return this;

        async void Impl()
        {
            while (!stopped)
            {
                await Task.Delay(interval);
                if (!stopped)
                {
                    callback();
                }
            }
        }
    }

    public ITimedCall In(TimeSpan interval)
    {
        Impl();
        return this;

        async void Impl()
        {
            await Task.Delay(interval);
            if (!stopped)
            {
                callback();
            }
        }
    }

    public void Stop()
    {
        stopped = true;
    }
}