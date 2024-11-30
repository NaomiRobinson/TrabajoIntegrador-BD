using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMainMenu : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Debug.Log("Volviendo al menú principal");
        GameManager.Instance.ResetQuestions();
        SceneManager.LoadScene("TriviaSelectScene");
    }
}
