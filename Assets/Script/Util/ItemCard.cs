using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    public Animator animator;

    public Text title;

    public Text des;

    [SerializeField] RawImage img;


    public void SetUpAndAnimationCard(ItemType itemType, bool isLeft)
    {
        title.text = ItemInfoProvider.GetDisplayName(itemType);
        des.text = ItemInfoProvider.GetDisplayName(itemType);
        img.texture = Resources.Load<Texture>($"Perks/{itemType}");

        if (isLeft)
        {
            animator.SetTrigger("ItemCardLeft");
        }
        else
        {
            animator.SetTrigger("PerkCardRight");
        }
    }
}