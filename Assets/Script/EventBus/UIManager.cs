using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument PerksAndShopUIDocument;
    [SerializeField] UIDocument MainBattle;


    private HitAnimation current;
    private void OnEnable()
    {
        EventBus.Subscribe<RoundOver>(PerksAndShopUIPOP);
        EventBus.Subscribe<HitAnimation>(HitAnimation);
        EventBus.Subscribe<SortHitEvent>(HitUi);
        EventBus.Subscribe<HardHitEvent>(HitUi);
    }

    

    private void OnDisable()
    {
        EventBus.Unsubscribe<RoundOver>(PerksAndShopUIPOP);
        EventBus.Unsubscribe<HitAnimation>(HitAnimation);
        EventBus.Unsubscribe<SortHitEvent>(HitUi);
        EventBus.Unsubscribe<HardHitEvent>(HitUi);
    }
    
    
    private void HitUi(SortHitEvent obj)
    {
        if (current.Player == Player.First)
        {
            Toast.ShowDamagePopupLeft( 2);
            //.Show("epallwl");
        }
        else
        {
            Toast.ShowDamagePopupRight(2);
            //Toast.Show("epallwl");
        }    
    }
    
    private void HitUi(HardHitEvent obj)
    {
        
        if (current.Player == Player.First)
        {
            Toast.ShowDamagePopupRight(2);
            Debug.Log("VAR");
        }
        else
        {
            Toast.ShowDamagePopupRight( 2);
            Debug.Log("VAR");

        }     
    }
    
    private void PerksAndShopUIPOP(RoundOver evt)
    {
        PerksAndShopUIDocument.enabled = true;
    }

    private void PerksAndShopUIDown(RoundOver evt)
    {
        PerksAndShopUIDocument.enabled = false;
    }


    private void HitAnimation(HitAnimation evt)
    {
        Debug.Log("hit animation" + "    " + evt.Player);
        if (evt.Player == Player.First)
        {
            Toast.ShowDamagePopupLeft(2);
            current = evt;
            //Toast.Show("epallwl");
        }
        else
        {
            Toast.ShowDamagePopupRight(2);
            current = evt;
            //Toast.Show("epallwl");
        }
    }
}