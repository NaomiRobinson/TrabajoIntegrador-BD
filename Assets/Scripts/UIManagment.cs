using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryText;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] public Button[] _buttons;

    public TextMeshProUGUI CategoryText => _categoryText;
    public TextMeshProUGUI QuestionText => _questionText;
    public Button[] Buttons => _buttons;

    private string _correctAnswer;
    private Color _originalButtonColor;
    private bool isLoadingQuestion = false;

    public static UIManagment Instance { get; private set; }

    public bool queryCalled;

    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

   private void Start()
{
    if (GameManager.Instance == null)
    {
        Debug.LogError("GameManager no está inicializado.");
        return; // Salir si GameManager no está listo
    }

    _originalButtonColor = _buttons[0].GetComponent<Image>().color;
    GameManager.Instance.answeredQuestions.Clear();
    LoadNextQuestion();
}
    public void OnButtonClick(int buttonIndex)
    {
        if (isLoadingQuestion) return; // Evita procesar clics mientras se carga una nueva pregunta

        string selectedAnswer = Buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        bool isCorrect = selectedAnswer == GameManager.Instance.GetCorrectAnswer();

        // Desactiva los botones para evitar más clics
        foreach (Button button in Buttons)
        {
            button.interactable = false;
        }

        if (isCorrect)
        {
            Debug.Log("Respuesta correcta!");
            ChangeButtonColor(buttonIndex, Color.green);
            Invoke("HandleCorrectAnswer", 2f);
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Inténtalo de nuevo.");
            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("HandleIncorrectAnswer", 2f);
        }
    }
    private void HandleCorrectAnswer()
    {
        RestoreButtonColor();
        GameManager.Instance._answers.Clear();
        LoadNextQuestion(); // Carga la siguiente pregunta
    }

    private void HandleIncorrectAnswer()
    {
        RestoreButtonColor();
        CallGameOver(); // Finaliza el juego
    }


    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        buttonImage.color = color;
    }

    private void RestoreButtonColor()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = _originalButtonColor;
        }
    }

    public void LoadNextQuestion()
    {
        if (GameManager.Instance.HasMoreQuestions())
        {
            Debug.Log("Loading next question...");
            GameManager.Instance.CategoryAndQuestionQuery();

        }
        else
        {
            Debug.Log("No hay más preguntas disponibles.");

            CallGameOver();
        }
    }



    public void PreviousScene()
    {
        GameManager.Instance.ResetQuestions();
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void UpdateUI(question selectedQuestion, List<string> answers)
    {
        if (Buttons == null || Buttons.Length == 0)
        {
            Debug.LogError("Buttons array is null or empty.");
            return;
        }

        CategoryText.text = PlayerPrefs.GetString("SelectedTrivia");
        QuestionText.text = selectedQuestion.QuestionText;

        for (int i = 0; i < Buttons.Length; i++)
        {
            if (i < answers.Count)
            {
                if (Buttons[i] != null)
                {
                    Buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[i];
                    Buttons[i].onClick.RemoveAllListeners();
                    int index = i;
                    Buttons[i].onClick.AddListener(() => OnButtonClick(index));
                    Buttons[i].gameObject.SetActive(true); // Asegúrate de que el botón esté activo
                    Buttons[i].interactable = true;
                }
                else
                {
                    Debug.LogWarning($"Button at index {i} is null.");
                }
            }
            else
            {
                Buttons[i].gameObject.SetActive(false); // Oculta el botón si no hay suficientes respuestas
            }
        }
    }

    private void CallGameOver()
    {
        GameManager.Instance.GameOver();
    }

}
