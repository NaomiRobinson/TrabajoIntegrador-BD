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
    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

    Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] private Button startButton;

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

        if (selectedIndex < 0 || selectedIndex >= trivias.Count)
        {
            Debug.LogError("El índice seleccionado no es válido.");
            return;
        }

        string selectedTrivia = _dropdown.options[selectedIndex].text;
        int triviaId = trivias[selectedIndex].id;

        Debug.Log($"Trivia seleccionada: {selectedTrivia}, ID: {triviaId}");

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex + 1);
        PlayerPrefs.SetString("SelectedTrivia", trivias[selectedIndex].category);
        PlayerPrefs.SetInt("selected_trivia_id", triviaId);

        // Reiniciar datos del juego ANTES de cargar la nueva escena
        GameManager.Instance.responseList = new List<question>();
        GameManager.Instance._numQuestionAnswered = 0;
        GameManager.Instance.answeredQuestions.Clear();

        // Cargar escena de forma asíncrona
        LoadMainSceneAsync();
    }
 private async void LoadMainSceneAsync()
{
    // Cargar la escena de forma asíncrona
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
    asyncLoad.allowSceneActivation = false; // No activar la escena inmediatamente

    // Espera hasta que la escena esté cargada
    while (!asyncLoad.isDone)
    {
        Debug.Log($"Cargando escena: {asyncLoad.progress * 100}% completado.");
        
        // Si la escena está completamente cargada (progress >= 0.9), activamos la escena
        if (asyncLoad.progress >= 0.9f)
        {
            Debug.Log("Escena cargada al 90%, esperando a que UIManagment esté disponible.");
            await WaitForUIManagment(); // Esperamos a que UIManagment esté disponible
            asyncLoad.allowSceneActivation = true; // Activamos la escena cuando UIManagment esté listo
        }

        await Task.Yield(); // Espera un frame antes de continuar
    }

    // Después de activar la escena, confirmamos
    Debug.Log("Escena activada.");
}

private async Task WaitForUIManagment()
{
    // Espera hasta que UIManagment esté disponible
    int attempts = 0; // Límite de intentos de espera
    while (UIManagment.Instance == null && attempts < 30) // Esperar hasta 30 frames
    {
        Debug.Log("Esperando a que UIManagment esté disponible...");
        await Task.Yield();  // Espera un frame
        attempts++;  // Incrementar el contador de intentos
    }

    if (UIManagment.Instance == null)
    {
        Debug.LogError("UIManagment no se inicializó correctamente en la escena 'Main'.");
        return;  // Salir si UIManagment no está disponible después de 30 intentos
    }

    // Una vez UIManagment esté disponible, reiniciamos el contador
    Debug.Log("UIManagment está disponible.");
    UIManagment.Instance.SetCorrectAnswerCount(0);
}




}
