using System;
using System.Collections.Generic;
using Unity.VisualScripting;
//202322158 이준상


/// <summary>
/// A centralized locator class that manages and provides ViewModel instances.
/// It handles the creation, initialization, and cleanup of ViewModels.
/// </summary>
public class ViewModelLocator
{
    // Dictionary acting as a storage for active ViewModel instances, mapped by their Type.
    private Dictionary<Type, ViewModelBase> _viewModels = new Dictionary<Type, ViewModelBase>();

    private static ViewModelLocator _instance;
    // Singleton: Ensures a single global access point for locating ViewModels.
    public static ViewModelLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ViewModelLocator();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Retrieves the ViewModel of the specified type.
    /// If it doesn't exist, it creates a new instance and initializes it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>() where T : ViewModelBase, new()
    {
        //You might wonder about the different initialization strategies.
        //Repositories are initialized at the very beginning of the game because they must be globally accessible at all times.
        //In contrast, ViewModels are lazily instantiated only when called.
        //This is because a ViewModel's lifecycle is tightly coupled with its corresponding View;
        //it should be created when the View starts and disposed of when the View closes.
        //That is why I opted for lazy loading for ViewModels.
        var type = typeof(T);
        if (!_viewModels.ContainsKey(type))
        {
            // Lazy initialization: create only when requested.
            
            var viewModel = new T();
            viewModel.Initialize(); // Runs the initialization logic immediately.
            _viewModels[type] = viewModel;
        }
        return (T)_viewModels[type];
    }

    /// <summary>
    /// Removes a ViewModel from the storage and triggers its cleanup logic.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Remove<T>() where T : ViewModelBase
    {
        var type = typeof(T);
        if (_viewModels.TryGetValue(type, out var vm))
        {
            // Calls Dispose to clear events and free up resources.
            vm.Dispose();
            _viewModels.Remove(type);
        }
    }
}