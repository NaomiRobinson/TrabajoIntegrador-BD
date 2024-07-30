using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    string supabaseUrl = "https://syrwwatmmjbsiyypndgd.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InN5cnd3YXRtbWpic2l5eXBuZGdkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjAwMjQ1OTUsImV4cCI6MjAzNTYwMDU5NX0.ogHLM4S-9PLIeCJgVWhUY8_ge2gQMooUNKr-J5QTXVA";

    Supabase.Client clientSupabase;

    public int index;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        index = PlayerPrefs.GetInt("SelectedIndex");

        await LoadTriviaData(index);
    }

    async Task LoadTriviaData(int index)
{
    var response = await clientSupabase
        .From<question>()
        .Where(question => question.trivia_id == index)
        .Select("id, question, answer1, answer2, answer3, correct_answer, trivia_id, trivia(id, category)")
        .Get();

    GameManager.Instance.currentTriviaIndex = index;
    GameManager.Instance.responseList = response.Models;

    Debug.Log("Response from query: " + response.Models.Count);
    Debug.Log("ResponseList from GM: " + GameManager.Instance.responseList.Count);

    // Comienza el juego solo después de haber cargado las preguntas
    if (response.Models.Count > 0)
    {
        UIManagment.Instance.LoadNextQuestion();
    }
}

}