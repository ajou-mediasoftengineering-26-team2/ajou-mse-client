using System;
using System.Collections.Generic;
//202322158 이준상
/// <summary>
/// A centralized event system that allows communication between objects without direct references.
/// Acts as a message broker using the Publish-Subscribe pattern.
/// </summary>
public static class EventBus
{
    // A dictionary that stores handlers for each event Type using Delegates.
    private static readonly Dictionary<Type, Delegate> _handlers = new();

    /// <summary>
    /// Subscribes a handler to a specific event type.
    /// </summary>
    /// <typeparam name="T">The type of the event to listen for.</typeparam>
    /// <param name="handler">The action to execute when the event is published.</param>
    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var existing))
            // If handlers already exist for this type, combine them.
            _handlers[type] = Delegate.Combine(existing, handler);
        else
            // If it's the first subscriber for this type, add it directly.
            _handlers[type] = handler;
    }

    /// <summary>
    /// Unsubscribes a handler from a specific event type.
    /// </summary>
    /// <param name="handler"></param>
    /// <typeparam name="T"></typeparam>
    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var existing)) return;

        // Removes the specific handler from the delegate chain.
        var updated = Delegate.Remove(existing, handler);
        
        if (updated == null) 
            // Remove the key from the dictionary if no subscribers remain.
            _handlers.Remove(type);
        else 
            // Update the dictionary with the remaining subscribers.
            _handlers[type] = updated;
    }

    /// <summary>
    /// Publishes an event to all active subscribers of that specific type.
    /// </summary>
    /// <typeparam name="T">The type of the event being broadcast.</typeparam>
    /// <param name="evt">The event data object to send.</param>
    public static void Publish<T>(T evt)
    {
        // Find and invoke all handlers registered for the published event type.
        if (_handlers.TryGetValue(typeof(T), out var del))
            (del as Action<T>)?.Invoke(evt);
    }
}