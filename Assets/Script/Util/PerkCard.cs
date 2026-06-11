using System;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

//202322158 이준상


/// <summary>
/// Perk Card Animation after LobbyState was GAME_TURN_ANIMATION
/// </summary>
public class PerkCard : MonoBehaviour
{
    public Animator animator;

    public Text title;

    public Text des;

    public RawImage img;



    /// <summary>
    /// Get Perk Information and Trigger Animation
    /// </summary>
    /// <param name="perkType"></param>
    /// <param name="isLeft"></param>
    public void SetUpAndAnimationCard(PerkType perkType, bool isLeft)
    {
        title.text = PerkInfoProvider.GetDisplayName(perkType);
        des.text = PerkInfoProvider.GetDescription(perkType);
        img.texture = Resources.Load<Texture>($"Perks/{perkType}");

        if (isLeft)
        {
            animator.SetTrigger("PerkCardLeft");   
        }
        else
        {
            animator.SetTrigger("PerkCardRight");
        }
    }
}
