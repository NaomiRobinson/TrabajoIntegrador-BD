using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public List<question> questionList;
    public List<int> answeredQuestions = new List<int>();
    public List<string> _answers = new List<string>();
    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;
    public int _numQuestionAnswered = 0;
    public string _correctAnswer;


    private int user_id;
    private int trivia_id;
    private int time;
    public int points = 10;
    public int questionPoints;
    public int triviaScore;
    private int timeUsed;

    public static GameManager Instance { get; private set; }


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
        // answeredQuestions.Clear();

        string selectedTrivia = PlayerPrefs.GetString("SelectedTrivia", "Valor por defecto");
        Debug.Log($"Trivia seleccionada: {selectedTrivia}");
    }

    //inicializacion de la pregunta 

    public void CategoryAndQuestionQuery()
    {
        if (questionList == null || questionList.Count == 0)
        {
            Debug.LogError("questionList está vacío o no ha sido cargado.");
            return; 
        }

        if (questionList.Count == answeredQuestions.Count)
        {
            Debug.Log("Todas las preguntas han sido respondidas.");
            GameOver();
            return;
        }

        int randomQuestionIndex;

        do
        {
            randomQuestionIndex = Random.Range(0, questionList.Count);
        }
  while (answeredQuestions.Contains(randomQuestionIndex));

        answeredQuestions.Add(randomQuestionIndex);

        question selectedQuestion = questionList[randomQuestionIndex];

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
            UIManagment.Instance.UpdateUI(selectedQuestion, _answers);
            UIManagment.Instance.ShowNextButton(false);
        }
        else
        {
            Debug.LogError("UIManagment no está inicializado.");
        }

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
        return answeredQuestions.Count < questionList.Count;
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

    public void GameOver()
    {

        //Obtencion de datos
        int user_id = SupabaseManager.CurrentUserId;
        int attempt_id = SupabaseManager.NewAttemptId;
        trivia_id = PlayerPrefs.GetInt("selected_trivia_id", 0);
        int correctAnswers = UIManagment.Instance.correct_answercount;
        int score = triviaScore;
        PlayerPrefs.SetInt("FinalScore", score);

    

        Debug.Log($"Tiempo al final: {time} segundos");
        Debug.Log($"user_id: {user_id}, trivia_id: {trivia_id}, score: {score}, correct_answercount: {correctAnswers}");

//Guarda intento
        if (user_id != 0 && trivia_id != 0)
        {
            SupabaseManager.Instance.SaveAttempt(attempt_id, user_id, trivia_id, score, correctAnswers);
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

        PlayerPrefs.SetString("GameOverMessage", "¡Que lastima!\nSe acabó el tiempo");

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

        PlayerPrefs.DeleteKey("SelectedTrivia");
        PlayerPrefs.DeleteKey("selected_trivia_id");

       
        ResetScore();
        Debug.Log("El juego ha sido reiniciado.");
    }

 

}