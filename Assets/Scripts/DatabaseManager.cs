using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

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
        if (clientSupabase == null)
        {
            Debug.LogError("Supabase client is not initialized.");
            return;
        }
        var response = await clientSupabase
            .From<question>()
            .Where(question => question.trivia_id == index)
            .Select("id, question, answer1, answer2, answer3, correct_answer, trivia_id, trivia(id, category)")
            .Get();

        if (response == null || response.Models.Count == 0)
        {
            Debug.LogError("No trivia data found for the given index.");
            return;
        }



        GameManager.Instance.currentTriviaIndex = index;
        GameManager.Instance.responseList = response.Models;
        Debug.Log("Response from query: " + response.Models.Count);
        Debug.Log("ResponseList from GM: " + GameManager.Instance.responseList.Count);




        UIManagment.Instance.LoadNextQuestion();

    }
} 