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
    private int score;

    public int _numQuestionAnswered = 0;
    public string _correctAnswer;
    public static GameManager Instance { get; private set; }



    public bool waitingForNext = false;

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

    }

    public void CategoryAndQuestionQuery()
    {
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

        // Actualiza UIManagment solo con la pregunta y respuestas
        if (UIManagment.Instance != null)
        {
            UIManagment.Instance.UpdateUI(selectedQuestion, _answers);
            UIManagment.Instance.ShowNextButton(false);
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
        //user_id = PlayerPrefs.GetInt("UserID", 0);
        int user_id = SupabaseManager.CurrentUserId;
        int attempt_id = SupabaseManager.NewAttemptId;
        trivia_id = PlayerPrefs.GetInt("selected_trivia_id", 0);
        int correctAnswers = UIManagment.Instance.correct_answercount;
        int score = ScoreManager.Instance.triviaScore;


        Debug.Log($"user_id: {user_id}, trivia_id: {trivia_id}, score: {score}, correct_answercount: {correctAnswers}, time: {time}");

        if (user_id != 0 && trivia_id != 0)
        {
            SupabaseManager.Instance.SaveAttempt(attempt_id, user_id, trivia_id, score, correctAnswers, time);
        }
        else
        {
            if (user_id == 0)
            {
                Debug.LogError("user_id no está configurado correctamente.");
            }
            if (trivia_id == 0)
            {
                Debug.LogError("trivia_id no está configurado correctamente.");
            }
        }

        SceneManager.LoadScene("GameOver");
    }

    public void TimeUp()
    {
        Debug.Log("El tiempo se ha agotado.");

        GameOver();
    }



}

