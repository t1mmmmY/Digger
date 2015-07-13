using UnityEngine;
using System.Collections;

public class TavernAnimationScript : MonoBehaviour 
{
    public static System.Action onEndAnimation;

	public void EndAnimation()
    {
        if (onEndAnimation != null)
        {
            onEndAnimation();
        }
    }
}
