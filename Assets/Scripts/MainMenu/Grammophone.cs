using UnityEngine;
using System.Collections;

public class Grammophone : MonoBehaviour 
{
    [SerializeField] Animator animator;
    int enableAnimationHash = Animator.StringToHash("enable");

    void Start()
    {
        animator.SetBool(enableAnimationHash, GeneralGameController.Instance.isMusicPlaying);
    }

    public void MuteAudio(bool isMusicPlaying)
    {
        animator.SetBool(enableAnimationHash, isMusicPlaying);
    }
}
