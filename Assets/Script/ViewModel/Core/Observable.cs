using System;
using System.Collections.Generic;

public class Observable<T>
{
    private T _value;
    public event Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }
    public void Subscribe(Action<T> action, bool invokeImmediately = true)
    {
        OnValueChanged -= action;
        OnValueChanged += action;
        if (invokeImmediately)
        {
            action?.Invoke(_value);
        }
    }

    public Observable(T initialValue = default)
    {
        _value = initialValue;
    }
}