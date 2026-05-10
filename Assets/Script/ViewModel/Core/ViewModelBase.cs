using System;
using System.Collections.Generic;
//202322158 이준상
/// <summary>
/// Abstract base class for all ViewModels.
/// Manages the initialization state and ensures proper cleanup via IDisposable.
/// </summary>
public abstract class ViewModelBase : IDisposable
{
    // Event triggered when the ViewModel is disposed of.
    public event Action OnDisposed;

    // Indicates whether the ViewModel has been initialized.
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Initializes the ViewModel. Includes logic to prevent redundant initialization.
    /// </summary>
    public virtual void Initialize()
    {
        if (IsInitialized) return;
        IsInitialized = true;
    }

    /// <summary>
    /// Cleans up resources and events when the object is no longer needed.
    /// </summary>
    public virtual void Dispose()
    {
        // Notifies all subscribers that this object is being disposed.
        OnDisposed?.Invoke();
        
        // Clear the event to prevent potential memory leaks.
        OnDisposed = null;
    }
}