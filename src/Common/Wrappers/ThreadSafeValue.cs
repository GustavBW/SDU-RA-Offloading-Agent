using System;
using System.Text.Json.Serialization;

namespace MonitoringSubsystem;

public class ThreadSafeValue<T>
{
    private T _value;
    private readonly object _mutex = new();

    public ThreadSafeValue(T value)
    {
        this._value = value;
    }

    public static ThreadSafeValue<T> Of(T value)
    {
        return new ThreadSafeValue<T>(value);
    }
    
    public T Get()
    {
        lock (_mutex)
        {
            return _value;
        }
    }

    public void Set(T value)
    {
        lock (_mutex)
        {
            this._value = value;
        }
    }
}