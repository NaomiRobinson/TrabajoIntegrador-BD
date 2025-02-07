using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public static int CurrentUserId { get; private set; }
    [SerializeField] private Button _playbutton;
    [SerializeField] private Button _exitbutton;
    [SerializeField] private Button _rankingbutton;
    [SerializeField] TextMeshProUGUI _userText;


    void Start()
    {
        if (_userText != null)
        {
            _userText.text = $"¡Hola {SupabaseManager.CurrentUserName}!";
        }


        if (_playbutton != null)
        {
            _playbutton.onClick.AddListener(NextScene);
        }

        if (_exitbutton != null)
        {
            _exitbutton.onClick.AddListener(QuitGame);
        }

        if (_rankingbutton != null)
        {
            _rankingbutton.onClick.AddListener(GoToRanking);
        }
    }
    void Update()
    { }
    public void NextScene()
    {
        SceneManager.LoadScene("TriviaSelectScene");
    }
    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }

    public void GoToRanking()
    {
        SceneManager.LoadScene("RankingScene");
    }
}
