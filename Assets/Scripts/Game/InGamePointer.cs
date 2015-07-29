using UnityEngine;
using System.Collections;

public class InGamePointer : MonoBehaviour 
{
    [SerializeField] Animator animator;
    int clickAnimationHash = Animator.StringToHash("Click");

    bool enable = true;

    void OnEnable()
    {
        enable = true;
        FormulaDrawer.OnAnswer += OnAnswer;
    }

    void OnDisable()
    {
        FormulaDrawer.OnAnswer -= OnAnswer;
    }

    void OnAnswer(bool result)
    {
        enable = false;
    }

    void OnMouseUp()
    {
        if (!enable)
        {
            return;
        }

        animator.SetTrigger(clickAnimationHash);
        //Debug.Log("Click");
    }

    public void LoadLevel()
    {
        LevelLoader.Instance.LoadLevel(Scene.Tavern);
    }
}
