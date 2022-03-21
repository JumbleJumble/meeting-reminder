using System;
using System.Threading.Tasks;

namespace MeetingReminder.Services;

public interface IIntervalTimer
{
    ITimedCall Call(Action callback);
}

public class IntervalTimer : IIntervalTimer
{
    public ITimedCall Call(Action callback)
    {
        return new TimedCall(callback);
    }
}

public interface ITimedCall
{
    ITimedCall In(TimeSpan interval);
    ITimedCall At(DateTimeOffset when);
    void Stop();
}

public class TimedCall : ITimedCall
{
    private readonly Action callback;
    private bool stopped;

    public TimedCall(Action callback)
    {
        this.callback = callback;
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

    public ITimedCall At(DateTimeOffset when)
    {
        return In(when - DateTimeOffset.Now);
    }

    public void Stop()
    {
        stopped = true;
    }
}
