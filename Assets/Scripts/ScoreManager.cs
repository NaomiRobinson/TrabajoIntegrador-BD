using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int points = 10;
    public int questionPoints;
    public int triviaScore;
    private int timeUsed;
    [SerializeField] GameObject rankingItemPrefab;
    [SerializeField] Transform contentTransform;
    [SerializeField] TMP_Dropdown rankingDropdown;
    [SerializeField] private Button backButton;
    [SerializeField] GameObject loadingSpinner;

    private List<trivia> trivias;

    private const int GeneralCategoryIndex = 0;

    public static ScoreManager Instance { get; private set; }



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

    async void Start()
    {
        await LoadTrivias();
        PopulateRankingDropdown();
        rankingDropdown.onValueChanged.AddListener(OnRankingDropdownValueChanged);

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackMenu);
        }

        await ShowRanking();
    }

    void Update()
    {
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


    public void PopulateRankingDropdown()
    {
        rankingDropdown.ClearOptions();

        List<string> options = new List<string> { "General" };

        foreach (var trivia in SupabaseManager.Instance.trivias)
        {
            if (!string.IsNullOrEmpty(trivia.category))
            {
                options.Add(trivia.category);
            }
        }

        Debug.Log("Opciones del Dropdown: " + string.Join(", ", options));
        rankingDropdown.AddOptions(options);
    }

    public async void OnRankingDropdownValueChanged(int index)
    {
        string selectedCategory = rankingDropdown.options[index].text;
        Debug.Log("Categoría seleccionada: " + selectedCategory);  // Verifica qué categoría se seleccionó

        int? triviaId = selectedCategory == "General" ? (int?)null : SupabaseManager.Instance.GetTriviaIdFromCategory(selectedCategory);
        await ShowRanking(triviaId);

        // Aquí debes filtrar e actualizar la lista con los puntajes filtrados

    }

    public int GetTriviaIdFromCategory(string category)
    {
        if (trivias == null || trivias.Count == 0)
        {
            Debug.LogError("Error: No hay trivias cargadas.");
            return -1;
        }

        var trivia = trivias.FirstOrDefault(t => t.category == category);
        return trivia != null ? trivia.id : -1;
    }


    public void CreateRanking(List<attempt> attempts)
    {

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log($"Mostrando {attempts.Count} intentos en el ranking.");

        int position = 1;
        foreach (var attempt in attempts)
        {
            GameObject rankingItem = Instantiate(rankingItemPrefab, contentTransform);
            TextMeshProUGUI[] texts = rankingItem.GetComponentsInChildren<TextMeshProUGUI>();

            // Buscar el usuario por su ID en la lista de usuarios cargados
            var user = SupabaseManager.Instance.users.FirstOrDefault(u => u.id == attempt.users_id);
            string username = user != null ? user.username : "Desconocido";

            texts[0].text = position.ToString();
            texts[1].text = username; // Mostrar el nombre de usuario
            texts[2].text = attempt.score.ToString(); // Asegúrate de convertir el score a string si es necesario

            position++;
        }
    }

    public async Task LoadTrivias()
    {
        var triviaData = await SupabaseManager.Instance.GetClientSupabase()
            .From<trivia>()
            .Select("*")
            .Get();

        trivias = triviaData.Models;

        Debug.Log("Trivias cargadas:");
        foreach (var t in trivias)
        {
            Debug.Log($"ID: {t.id}, Categoría: {t.category}");
        }
        PopulateRankingDropdown();
    }
    public async Task ShowRanking(int? triviaId = null)
    {
        Debug.Log("ShowRanking ha sido llamada.");
        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(true);
        }

        List<attempt> filteredAttempts = await SupabaseManager.Instance.OrderScore(triviaId);
        Debug.Log($"Número de intentos filtrados para trivia {triviaId}: {filteredAttempts.Count}");
        foreach (var attempt in filteredAttempts)
        {
            Debug.Log($"Intento - Usuario: {attempt.users_id}, Puntuación: {attempt.score}, Trivia ID: {attempt.trivia_id}");
        }

        CreateRanking(filteredAttempts);

        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(false);
        }
    }


    public void BackMenu()
    {
        rankingDropdown.onValueChanged.RemoveListener(OnRankingDropdownValueChanged);
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }



}
