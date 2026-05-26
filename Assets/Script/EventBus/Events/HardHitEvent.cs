using UnityEngine.UIElements;

public readonly struct HardHitEvent
{
    public readonly VisualElement visualElement;
    
    public HardHitEvent(VisualElement visualElement)
    {
        this.visualElement = visualElement;
    }
}