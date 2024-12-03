using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMainMenu : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Debug.Log("Volviendo al menú principal");
        Destroy(GameManager.Instance);
        SceneManager.LoadScene("TriviaSelectScene");
    }
}
