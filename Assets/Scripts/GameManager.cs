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
    public int _numQuestionAnswered = 0;
    public string _correctAnswer;

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
        answeredQuestions.Clear();
        queryCalled = false;
        Debug.Log($"answeredQuestions inicial: {answeredQuestions.Count}");

    }

    public void CategoryAndQuestionQuery()
    {
        if (responseList.Count == answeredQuestions.Count)
        {
            Debug.Log("Todas las preguntas han sido respondidas.");
            Debug.Log($"responseList.Count: {responseList.Count}, answeredQuestions.Count: {answeredQuestions.Count}");
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
        }

    }



    public void ResetQuestions()
    {
        answeredQuestions.Clear();
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
        SceneManager.LoadScene("GameOver");
    }
}

