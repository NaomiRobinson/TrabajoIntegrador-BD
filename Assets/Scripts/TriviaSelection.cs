using UnityEngine;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{


    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] private Button startButton;

    public DatabaseManager databaseManager;

 void Start()
    {
        PopulateDropdown();
    }

    

    void PopulateDropdown()
{
    _dropdown.ClearOptions();
    
    List<string> categories = new List<string>();

    foreach (var trivia in SupabaseManager.Instance.trivias) 
    {
        categories.Add(trivia.category);
    }

    _dropdown.AddOptions(categories);
}

   public void OnStartButtonClicked()
    {
        int selectedIndex = _dropdown.value;

        if (selectedIndex < 0 || selectedIndex >= SupabaseManager.Instance.trivias.Count)
        {
            Debug.LogError("El índice seleccionado no es válido.");
            return;
        }

        string selectedTrivia = _dropdown.options[selectedIndex].text;
        int triviaId = SupabaseManager.Instance.trivias[selectedIndex].id;

        Debug.Log($"Trivia seleccionada: {selectedTrivia}, ID: {triviaId}");

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex + 1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);
        PlayerPrefs.SetInt("selected_trivia_id", triviaId);

        // Reiniciar datos del juego 
        GameManager.Instance.questionList = new List<question>();
        GameManager.Instance._numQuestionAnswered = 0;
        GameManager.Instance.answeredQuestions.Clear();

        SceneManager.LoadScene("Main");
    }


}
