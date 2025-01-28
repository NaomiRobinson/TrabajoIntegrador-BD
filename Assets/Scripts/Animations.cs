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
        animatorImage.Play("Idle");
         hasStarted = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void QuestionHasImage(bool hasImage)
    {


        if (!hasStarted) return; // Evita que se ejecute durante la inicialización

    ResetAllTriggers();

    if (hasImage)
    {
        animator1.SetTrigger("MoveButton");
        animator2.SetTrigger("MoveButton");
        animator3.SetTrigger("MoveButton");

        animatorImage.SetTrigger("ShowImage");
    }
    else
    {
        animator1.SetTrigger("ResetButton");
        animator2.SetTrigger("ResetButton");
        animator3.SetTrigger("ResetButton");

        animatorImage.SetTrigger("HideImage");
    }


    }

    private void ResetAllTriggers()
    {

        animator1.ResetTrigger("MoveButton");
        animator2.ResetTrigger("MoveButton");
        animator3.ResetTrigger("MoveButton");
        animator1.ResetTrigger("ResetButton");
        animator2.ResetTrigger("ResetButton");
        animator3.ResetTrigger("ResetButton");
        animatorImage.ResetTrigger("ShowImage");
        animatorImage.ResetTrigger("HideImage");
    }



}




