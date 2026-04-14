using System;
using System.Collections.Generic;


public class RepositoryFactory
{
    private static RepositoryFactory _instance;
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

    private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

    public void Register<TInterface, TImplementation>() 
        where TInterface : class 
        where TImplementation : class, TInterface, new()
    {
        _repositories[typeof(TInterface)] = new TImplementation();
    }

    
    public TInterface Get<TInterface>() where TInterface : class
    {
        Type type = typeof(TInterface);
        if (_repositories.ContainsKey(type))
        {
            return _repositories[type] as TInterface;
        }
        
        throw new InvalidOperationException($"Repository {type.Name} is not registered.");
    }

    public void Clear()
    {
        _repositories.Clear();
    }
}

