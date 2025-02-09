using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public List<question> responseList; // Lista de preguntas
    public List<int> answeredQuestions = new List<int>();

    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;

    public List<string> _answers = new List<string>();

    public bool queryCalled;

    private int _points;

    private int user_id;

    private int trivia_id;

    private int time;

    private Timer timer;

    public int _numQuestionAnswered = 0;
    public string _correctAnswer;
    public static GameManager Instance { get; private set; }
    public bool waitingForNext = false;

    public int points = 10;
    public int questionPoints;
    public int triviaScore;
    private int timeUsed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
       
        answeredQuestions.Clear();
        queryCalled = false;
        Debug.Log($"answeredQuestions inicial: {answeredQuestions.Count}");
        Debug.Log($"user_id: {user_id}");

        string selectedTrivia = PlayerPrefs.GetString("SelectedTrivia", "Valor por defecto");
        Debug.Log($"Trivia seleccionada: {selectedTrivia}");


    }
    public void CategoryAndQuestionQuery()
    {
        if (responseList == null || responseList.Count == 0)
        {
            Debug.LogError("responseList está vacío o no ha sido cargado.");
            return; // Salir del método si la lista está vacía
        }

        if (responseList.Count == answeredQuestions.Count)
        {
            Debug.Log("Todas las preguntas han sido respondidas.");
            GameOver();
            return;
        }

        int randomQuestionIndex;
        do
        {
            randomQuestionIndex = Random.Range(0, responseList.Count);
        }
        while (answeredQuestions.Contains(randomQuestionIndex));

        answeredQuestions.Add(randomQuestionIndex);

        question selectedQuestion = responseList[randomQuestionIndex];
        _answers.Clear();
        _answers.Add(selectedQuestion.Answer1);
        _answers.Add(selectedQuestion.Answer2);
        _answers.Add(selectedQuestion.Answer3);

        _answers.Shuffle();
        _correctAnswer = selectedQuestion.CorrectOption;

        if (selectedQuestion == null)
        {
            Debug.LogError("selectedQuestion es nulo.");
            return;
        }

        if (_answers == null || _answers.Count == 0)
        {
            Debug.LogError("La lista de respuestas está vacía o es nula.");
            return;
        }

        if (UIManagment.Instance != null)
        {
            UIManagment.Instance.UpdateUI(selectedQuestion, _answers);  // Cambio: Ahora actualizamos la UI desde GameManager
            UIManagment.Instance.ShowNextButton(false); 
        }
        else
        {
            Debug.LogError("UIManagment no está inicializado.");
        }

        waitingForNext = false;

        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.ResetTimer();
        }
    }
    
    public string GetCorrectAnswer()
    {
        return _correctAnswer;
    }
    public bool HasMoreQuestions()
    {
        return answeredQuestions.Count < responseList.Count;
    }
    public void GameOver()
{
    int user_id = SupabaseManager.CurrentUserId;
    int attempt_id = SupabaseManager.NewAttemptId;
    trivia_id = PlayerPrefs.GetInt("selected_trivia_id", 0);
    int correctAnswers = UIManagment.Instance.correct_answercount;
    
    int score = triviaScore;
      PlayerPrefs.SetInt("FinalScore", score); 

    if (Timer.Instance != null)
    {
        time = (int)Timer.Instance.GetGameTime();
    }

    Debug.Log($"Tiempo al final: {time} segundos");
    Debug.Log($"user_id: {user_id}, trivia_id: {trivia_id}, score: {score}, correct_answercount: {correctAnswers}, time: {time}");

    if (user_id != 0 && trivia_id != 0)
    {
        SupabaseManager.Instance.SaveAttempt(attempt_id, user_id, trivia_id, score, correctAnswers, time);
    }
    else
    {
        if (user_id == 0) { Debug.LogError("user_id no está configurado correctamente."); }
        if (trivia_id == 0) { Debug.LogError("trivia_id no está configurado correctamente."); }
    }

    SceneManager.LoadScene("GameOver");
}

    public void TimeUp()
    {
        Debug.Log("El tiempo se ha agotado.");

        GameOver();
    }

    public void ResetGame()
    {
        answeredQuestions.Clear();
        currentTriviaIndex = 0;
        randomQuestionIndex = 0;
        _answers.Clear();
        _numQuestionAnswered = 0;
        _correctAnswer = "";
        queryCalled = false;
        waitingForNext = false;


        PlayerPrefs.DeleteKey("SelectedTrivia");
        PlayerPrefs.DeleteKey("selected_trivia_id");

        Timer.Instance?.ResetGamerTimer();
       ResetScore();

        Debug.Log("El juego ha sido reiniciado.");
    }

      public void CalculateScore(int timeLeft, int maxTime)
{
    Debug.Log("Calculando puntaje...");
    timeUsed = maxTime - timeLeft;
    questionPoints = Mathf.Max(0, points - timeUsed);
    triviaScore += questionPoints;
    Debug.Log($"Puntos obtenidos en esta pregunta: {questionPoints}");
    Debug.Log($"Puntaje total acumulado: {triviaScore}");
}


 public void ResetScore()
{
    triviaScore = 0;
    questionPoints = 0;
    Debug.Log("Puntaje reiniciado.");
}

}