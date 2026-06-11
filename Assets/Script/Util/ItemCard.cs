using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

//2023322158 이준상
/// <summary>
/// Item Card Animation after LobbyState was GAME_TURN_ANIMATION
/// </summary>
public class ItemCard : MonoBehaviour
{
    public Animator animator;

    public Text title;

    public Text des;

    [SerializeField] RawImage img;


    /// <summary>
    /// Get Item Information and Trigger Animation
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="isLeft"></param>
    public void SetUpAndAnimationCard(ItemType itemType, bool isLeft)
    {
        title.text = ItemInfoProvider.GetDisplayName(itemType);
        des.text = ItemInfoProvider.GetDescription(itemType);
        img.texture = Resources.Load<Texture>($"Items/{itemType}");

        if (isLeft)
        {
            animator.SetTrigger("ItemCardLeft");
        }
        else
        {
            animator.SetTrigger("ItemCardRight");
        }
    }
}