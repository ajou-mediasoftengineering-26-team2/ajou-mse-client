using System;
using System.Collections.Generic;


//202322158 이준상

/// <summary>
/// A wrapper class that notifies listeners whenever the internal value changes.
/// Implements a simple Observer pattern.
/// I made a class by referring to the lecture materials from Professor Kyung Min Ho's OOP class.
/// </summary>
public class Observable<T>
{
    protected T _value;
    // Event that triggers whenever the value is updated.
    public event Action<T> OnValueChanged;

    /// <summary>
    /// Gets or sets the current value.
    /// Notifications are sent only if the new value is different.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            // Check if the value has actually changed to prevent redundant updates.
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                // Invoke all registered listeners (subscribers) with the new value.
                Notify(_value);
            }
        }
    }
    
    protected void Notify(T value)
    {
        OnValueChanged?.Invoke(value);
    }

    /// <summary>
    /// Registers a callback function to be notified of value changes.
    /// </summary>
    /// <param name="action">The callback function to register.</param>
    /// <param name="invokeImmediately">If true, executes the callback immediately with the current value.</param>
    public void Subscribe(Action<T> action, bool invokeImmediately = true)
    {
        // Prevents duplicate subscriptions by removing the action before adding it.
        // If you only use +, the action will be duplicate.
        OnValueChanged -= action;
        OnValueChanged += action;
        
        // Useful for initializing UI elements with the current value upon subscription.
        if (invokeImmediately)
        {
            action?.Invoke(_value);
        }
    }

    // Constructor to set the initial value of the observable.
    public Observable(T initialValue = default)
    {
        _value = initialValue;
    }
}