using UnityEngine;
using System.Collections;

public class DailyBonusHintBubble : MonoBehaviour 
{
    [SerializeField] Animator bubbleAnimator;
    int showBubbleHash = Animator.StringToHash("ShowLabel");

    void OnEnable()
    {
        if (BonusController.Instance.IsBonusReady())
        {
            bubbleAnimator.SetTrigger(showBubbleHash);
        }
    }
}
