using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상
public class MainBattleDotsRenderer
{
    private readonly VisualTreeAsset _roundItemTemplate;
    private readonly List<VisualElement> _myDots = new();
    private readonly List<VisualElement> _enemyDots = new();

    public MainBattleDotsRenderer(VisualTreeAsset roundItemTemplate)
    {
        _roundItemTemplate = roundItemTemplate;
    }

    public void Initialize(VisualElement myContainer, VisualElement enemyContainer)
    {
        InitializeDots(myContainer, _myDots);
        InitializeDots(enemyContainer, _enemyDots);
    }

    public void RefreshMy(int currentWins) => RefreshDots(_myDots, currentWins);
    public void RefreshEnemy(int currentWins) => RefreshDots(_enemyDots, currentWins);

    private void InitializeDots(VisualElement container, List<VisualElement> cacheList)
    {
        if (container == null || _roundItemTemplate == null)
        {
            Debug.LogError("InitializeDots failed: container or round template is null.");
            return;
        }

        container.Clear();
        cacheList.Clear();

        for (int i = 0; i < GameSetting.ROUNDWINING; i++)
        {
            var item = _roundItemTemplate.Instantiate();
            item.style.flexDirection = FlexDirection.Row;
            item.style.marginRight = 5;
            item.style.marginLeft = 5;
            container.Add(item);

            var dot = item.Q<VisualElement>("Dot");
            if (dot != null) cacheList.Add(dot);
        }
    }

    private static void RefreshDots(List<VisualElement> dots, int currentWins)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].EnableInClassList("round-dot--active", i < currentWins);
        }
    }
}
