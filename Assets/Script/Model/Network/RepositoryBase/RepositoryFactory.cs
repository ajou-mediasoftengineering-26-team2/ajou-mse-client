using System;
using System.Collections.Generic;


//202322158 이준상
public class RepositoryFactory
{
    private static RepositoryFactory _instance;
    // Singleton: Ensures a single global access point to the repository container.
    public static RepositoryFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RepositoryFactory();
            }
            return _instance;
        }
    }

    // A dictionary that stores instantiated objects, mapped by their Interface Type.
    private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

    /// <summary>
    /// Registers a link between an interface and its concrete implementation.
    /// </summary>
    /// <typeparam name="TInterface">The interface to register (e.g., IItemRepository)</typeparam>
    /// <typeparam name="TImplementation">The actual class that implements it (e.g., ItemRepository)</typeparam>
    public void Register<TInterface, TImplementation>() 
        where TInterface : class 
        where TImplementation : class, TInterface, new()
    {
        // Map the interface type to a new instance of the implementation.
        _repositories[typeof(TInterface)] = new TImplementation();
    }

    /// <summary>
    /// Retrieves the registered implementation for a specific interface from the container.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TInterface Get<TInterface>() where TInterface : class
    {
        Type type = typeof(TInterface);
        if (_repositories.ContainsKey(type))
        {
            // Cast the stored object back to the requested interface.
            return _repositories[type] as TInterface;
        }
        
        // Throws an exception if the requested interface hasn't been registered yet.
        throw new InvalidOperationException($"Repository {type.Name} is not registered.");
    }

    /// <summary>
    /// Clears all registered repositories from the container.
    /// </summary>
    public void Clear()
    {
        _repositories.Clear();
    }
}

