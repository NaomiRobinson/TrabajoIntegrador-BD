using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{
    string supabaseUrl = "https://syrwwatmmjbsiyypndgd.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InN5cnd3YXRtbWpic2l5eXBuZGdkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjAwMjQ1OTUsImV4cCI6MjAzNTYwMDU5NX0.ogHLM4S-9PLIeCJgVWhUY8_ge2gQMooUNKr-J5QTXVA"; //COMPLETAR

    Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;

    public DatabaseManager databaseManager;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();
    }

    async Task SelectTrivias()
    {
        var response = await clientSupabase
            .From<trivia>()
            .Select("*")
            .Get();

        if (response != null)
        {
            trivias = response.Models;
        }
    }

    void PopulateDropdown()
    {
        _dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _dropdown.AddOptions(categories);
    }

    public void OnStartButtonClicked()
    {
        int selectedIndex = _dropdown.value;
        string selectedTrivia = _dropdown.options[selectedIndex].text;

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex + 1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);

        SceneManager.LoadScene("Main");
    }

}
