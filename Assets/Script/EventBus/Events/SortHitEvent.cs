using UnityEngine.UIElements;

public readonly struct SortHitEvent
{
    public readonly VisualElement visualElement;

    public SortHitEvent(VisualElement visualElement)
    {
        this.visualElement = visualElement;
    }
}