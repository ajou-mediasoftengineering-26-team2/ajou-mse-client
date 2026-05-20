using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument PerksAndShopUIDocument;
    private void OnEnable()
    {
        EventBus.Subscribe<RoundOver>(PerksAndShopUIPOP);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<RoundOver>(PerksAndShopUIPOP);
    }
    
    private void PerksAndShopUIPOP(RoundOver evt)
    {
        PerksAndShopUIDocument.enabled = true;
    }

    private void PerksAndShopUIDown(RoundOver evt)
    {
        PerksAndShopUIDocument.enabled = false;
    }
}