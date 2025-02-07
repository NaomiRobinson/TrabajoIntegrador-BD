using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ReturnMainMenu : MonoBehaviour
{

    [SerializeField] private Button _menuButton;

    void Start()
    {
        if (_menuButton != null)
        {
            _menuButton.onClick.AddListener(LoadMainMenu);
        }

    }
    public void LoadMainMenu()
    {

        SceneManager.LoadScene("MainMenu");
    }

}
