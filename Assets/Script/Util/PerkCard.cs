using System;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class PerkCard : MonoBehaviour
{
    public Animator animator;

    public Text title;

    public Text des;

    public RawImage img;



    public void SetUpAndAnimationCard(PerkType perkType, bool isLeft)
    {
        title.text = PerkInfoProvider.GetDisplayName(perkType);
        des.text = PerkInfoProvider.GetDisplayName(perkType);
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
