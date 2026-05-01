using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _handlers =
        new();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var existing))
            _handlers[type] = Delegate.Combine(existing, handler);
        else
            _handlers[type] = handler;
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var existing)) return;

        var updated = Delegate.Remove(existing, handler);
        if (updated == null) _handlers.Remove(type);
        else _handlers[type] = updated;
    }

    public static void Publish<T>(T evt)
    {
        if (_handlers.TryGetValue(typeof(T), out var del))
            (del as Action<T>)?.Invoke(evt);
    }
}