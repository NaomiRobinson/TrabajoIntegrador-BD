using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
    public Animator animatorImage;

    private bool hasStarted = false;

    private void Awake()
    {
        if (animator1 == null || animator2 == null || animator3 == null || animatorImage == null)
        {
            Debug.LogWarning("Uno o más Animators no están asignados.");
        }
    }

    void Start()
    {
        if (animatorImage.HasState(0, Animator.StringToHash("Idle")))
        {
            animatorImage.Play("Idle");
        }
        else
        {
            Debug.LogError("El estado 'Idle' no existe en el AnimatorController.");
        }
        hasStarted = true;
    }

    public void QuestionHasImage(bool hasImage)
    {

    if (!hasStarted) return; 
    
       if (hasImage)
    {
        ResetAllTriggers();
        animator1.SetTrigger("MoveButton");
        animator2.SetTrigger("MoveButton");
        animator3.SetTrigger("MoveButton");
        animatorImage.SetTrigger("ShowImage");
    }
    else
    {
        ResetAllTriggers();
        animator1.SetTrigger("ResetButton");
        animator2.SetTrigger("ResetButton");
        animator3.SetTrigger("ResetButton");
        animatorImage.SetTrigger("HideImage");
    }

    }

    private void ResetAllTriggers()
    {
        string[] triggers = { "MoveButton", "ResetButton"};

        foreach (var trigger in triggers)
    {
        animator1.ResetTrigger(trigger);
        animator2.ResetTrigger(trigger);
        animator3.ResetTrigger(trigger);

        animatorImage.ResetTrigger("ShowImage");
        animatorImage.ResetTrigger("HideImage");
    }
    }

}




