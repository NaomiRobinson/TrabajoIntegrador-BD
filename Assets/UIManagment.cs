using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] TextMeshProUGUI _questionText;

    [SerializeField] Button _nextButton;
    [SerializeField] Button _backButton;
    [SerializeField] Timer timer;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] Image questionImage;
    [SerializeField] DatabaseManager databaseManager;
    [SerializeField] Animations animations;


    public TextMeshProUGUI CategoryText => _categoryText;
    public TextMeshProUGUI QuestionText => _questionText;
    public Button[] _buttons = new Button[3];

    public static UIManagment Instance { get; private set; }


    private Color _originalButtonColor;
    private bool isLoadingQuestion = false;

    public int correct_answercount { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }

        if (_buttons == null || _buttons.Length == 0)
        {
            Debug.LogError("Los botones no están asignados correctamente en el inspector.");
            return;
        }
        if (_nextButton != null)
        {
            _nextButton.onClick.AddListener(LoadNextQuestion);
        }
        // if (_backButton != null)
        // {
        //     _backButton.gameObject.SetActive(false);
        // }
        // _backButton.onClick.AddListener(PreviousScene);
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no está inicializado.");
            return;
        }

        string selectedTrivia = PlayerPrefs.GetString("SelectedTrivia", "Valor por defecto");
        Debug.Log($"SelectedTrivia: {selectedTrivia}");

        CategoryText.text = selectedTrivia;
        //correct_answercount = 0;

        _originalButtonColor = _buttons[0].GetComponent<Image>().color;
        GameManager.Instance.answeredQuestions.Clear();
        // LoadNextQuestion();
        Debug.Log($"UIManagment Instance: {Instance}");

        UpdateScoreUI(GameManager.Instance.triviaScore, GameManager.Instance.questionPoints);

    }
    public void OnButtonClick(int buttonIndex)
    {
        if (isLoadingQuestion) return; // Evita procesar clics mientras se carga una nueva pregunta

        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        bool isCorrect = selectedAnswer == GameManager.Instance.GetCorrectAnswer();

        // Desactiva los botones para evitar más clics
        foreach (Button button in _buttons)
        {
            button.interactable = false;
        }

        if (isCorrect)
        {
            Debug.Log("Respuesta correcta!");
            ChangeButtonColor(buttonIndex, Color.green);
            if (timer != null)
            {
                timer.PauseTimer();
            }
            correct_answercount++;
            Invoke("HandleCorrectAnswer", 0.5f);
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Inténtalo de nuevo.");
            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("HandleIncorrectAnswer", 0.5f);
        }
    }
    private void HandleCorrectAnswer()
    {
        GameManager.Instance._answers.Clear();
        UpdateScoreUI(GameManager.Instance.triviaScore, GameManager.Instance.questionPoints);
        ShowNextButton(true);
    }

    private void HandleIncorrectAnswer()
    {
        RestoreButtonColor();
        CallGameOver();
        PlayerPrefs.SetString("GameOverMessage", "Respuesta incorrecta\nIntentalo de nuevo");
    }


    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }

    private void RestoreButtonColor()
    {
        Color defaultColor = new Color(0.992f, 1f, 0.863f, 1f);

        if (_buttons == null || _buttons.Length == 0)
        {
            Debug.LogError("Los botones no están asignados correctamente.");
            return;
        }

        foreach (Button button in _buttons)
        {
            if (button != null) // Asegurar que el botón sigue existiendo
            {
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = defaultColor; // Restablecer color
                    button.interactable = true; // Asegurarse de que los botones sean interactivos
                    button.gameObject.SetActive(true); // Asegurarse de que los botones estén visibles
                }
            }
        }
    }

    public void ShowNextButton(bool show)
    {
        if (_nextButton != null)
        {
            _nextButton.gameObject.SetActive(show);
        }
    }

    public void LoadNextQuestion()
    {
        if (GameManager.Instance.HasMoreQuestions())
        {
            Debug.Log("Cargando la siguiente pregunta...");
            GameManager.Instance.CategoryAndQuestionQuery();
            RestoreButtonColor();
            ShowNextButton(false);

            if (timer != null)
            {
                timer.ResetTimer();
            }
        }
        else
        {
            Debug.Log("No hay más preguntas disponibles.");
            PlayerPrefs.SetString("GameOverMessage", "¡Felicidades!\nRespondiste todas las preguntas.");
            CallGameOver();
        }
    }

    // public void PreviousScene()
    // {

    //     Destroy(GameManager.Instance);
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    // }

    public void UpdateUI(question selectedQuestion, List<string> answers)
    {

        if (_buttons == null || _buttons.Length == 0)
        {
            Debug.LogError("El array de _buttons es null o vacío.");
            return;
        }

        CategoryText.text = PlayerPrefs.GetString("SelectedTrivia");
        QuestionText.text = selectedQuestion.QuestionText;


        // Manejo de la imagen

        string assetUrl = selectedQuestion.asset;

        bool hasImage = !string.IsNullOrEmpty(assetUrl);

        if (hasImage)
        {
            if (databaseManager != null)
            {
                questionImage.sprite = null; //limpiar imagen previa
                StartCoroutine(databaseManager.LoadImage(assetUrl, null));
            }

        }
        else
        {
            questionImage.gameObject.SetActive(false);
        }

        animations.QuestionHasImage(hasImage);

        // Actualiza los botones con las respuestas
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] == null)
            {
                Debug.LogError($"Button at index {i} is null.");
                continue;
            }

            if (i < answers.Count)
            {
                var buttonText = _buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = answers[i];
                    _buttons[i].onClick.RemoveAllListeners(); // Limpiar los listeners previos
                    int index = i;
                    _buttons[i].onClick.AddListener(() => OnButtonClick(index)); // Asigna el nuevo listener
                    _buttons[i].gameObject.SetActive(true); // Asegura que el botón esté activo
                    _buttons[i].interactable = true; // Asegura que el botón sea interactivo
                }
            }
            else
            {
                _buttons[i].gameObject.SetActive(false); // Desactiva el botón si no hay respuesta
            }
        }
    }

    public void UpdateScoreUI(int score, int questionPoints)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }

        if (pointsText != null)
        {
            pointsText.gameObject.SetActive(true);
            pointsText.text = $"+{questionPoints}";
            Invoke("EffectPointsText", 1f);
        }

    }

    private void EffectPointsText()
    {
        pointsText.gameObject.SetActive(false);
    }

    private void CallGameOver()
    {
        GameManager.Instance.GameOver();
    }

}