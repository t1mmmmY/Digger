using UnityEngine;
using System.Collections;

public class Grammophone : MonoBehaviour 
{
    [SerializeField] Animator animator;
    int enableAnimationHash = Animator.StringToHash("enable");

    void Start()
    {
		if (GeneralGameController.Instance != null)
		{
        	animator.SetBool(enableAnimationHash, GeneralGameController.Instance.isMusicPlaying);
		}
		else
		{
			animator.SetBool(enableAnimationHash, true);
		}
    }

    public void MuteAudio(bool isMusicPlaying)
    {
        animator.SetBool(enableAnimationHash, isMusicPlaying);
    }
}
