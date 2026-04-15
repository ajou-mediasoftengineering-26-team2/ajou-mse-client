using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ViewModelLocator
{
    private Dictionary<Type, ViewModelBase> _viewModels = new Dictionary<Type, ViewModelBase>();

    private static ViewModelLocator _instance;
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
    // 뷰모델 가져오기 (없으면 생성)
    public T Get<T>() where T : ViewModelBase, new()
    {
        var type = typeof(T);
        if (!_viewModels.ContainsKey(type))
        {
            var viewModel = new T();
            viewModel.Initialize(); // 초기화 로직 실행
            _viewModels[type] = viewModel;
        }
        return (T)_viewModels[type];
    }

    public void Remove<T>() where T : ViewModelBase
    {
        var type = typeof(T);
        if (_viewModels.TryGetValue(type, out var vm))
        {
            vm.Dispose();
            _viewModels.Remove(type);
        }
    }
}