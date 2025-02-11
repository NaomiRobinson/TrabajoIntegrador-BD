using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ReturnMainMenu : MonoBehaviour
{

    [SerializeField] private Button _menuButton;
    [SerializeField] private TMP_Text finalScoreText;
     [SerializeField] private TMP_Text gameOverMessageText;

    void Start()
    {
        if (_menuButton != null)
        {
            _menuButton.onClick.AddListener(LoadMainMenu);
        }

        if (finalScoreText != null)
        {
            int finalScore = PlayerPrefs.GetInt("FinalScore", 0); 
            finalScoreText.text = $"Puntaje Final: {finalScore}"; 
        }

         if (gameOverMessageText != null)
        {
            gameOverMessageText.text = PlayerPrefs.GetString("GameOverMessage", "¡Juego terminado!");
        }

    }
    
    public void LoadMainMenu()
    { if (_menuButton != null)
    {
        _menuButton.onClick.RemoveListener(LoadMainMenu);
    }

    GameManager.Instance.ResetGame();
    SceneManager.LoadScene("MainMenu");
    }

}
