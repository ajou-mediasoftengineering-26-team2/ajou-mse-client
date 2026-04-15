using System;
using System.Collections.Generic;

public abstract class ViewModelBase : IDisposable
{
    public event Action OnDisposed;

    public bool IsInitialized { get; private set; }

    public virtual void Initialize()
    {
        if (IsInitialized) return;
        IsInitialized = true;
    }

    public virtual void Dispose()
    {
        OnDisposed?.Invoke();
        
        // 모든 구독자 제거 (메모리 누수 방지)
        OnDisposed = null;
    }
}