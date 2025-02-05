using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class ScoreManager : MonoBehaviour
{
    public int points = 10;
    public int questionPoints;
    public int triviaScore;
    private int timeUsed;
    [SerializeField] GameObject rankingItemPrefab;
    [SerializeField] Transform contentTransform;
    [SerializeField] TMP_Dropdown rankingDropdown;

    private List<trivia> trivias;

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

    void Start()
    {
        Instance.ShowRanking();
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


    void PopulateRankingDropdown()
    {
        rankingDropdown.ClearOptions();

        List<string> options = new List<string> { "General" };

        foreach (var trivia in trivias)
        {
            if (!string.IsNullOrEmpty(trivia.category))
            {
                options.Add(trivia.category);
            }
        }

        rankingDropdown.AddOptions(options);
    }


    public void CreateRanking(List<attempt> attempts)
    {

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        int position = 1;

        foreach (var attempt in attempts)
        {
            GameObject rankingItem = Instantiate(rankingItemPrefab, contentTransform);
            TextMeshProUGUI[] texts = rankingItem.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = "" + position;
            texts[1].text = attempt.user.username;
            texts[2].text = attempt.score;

            position++;
        }
    }

    public async void LoadTrivias()
    {
        var triviaData = await SupabaseManager.Instance.GetClientSupabase()
            .From<trivia>()
            .Select("*")
            .Get();

        trivias = triviaData.Models;
        PopulateRankingDropdown();
    }
    public async void ShowRanking()
    {
        Debug.Log("ShowRanking ha sido llamada.");
        await SupabaseManager.Instance.OrderScore();

        if (SupabaseManager.Instance.ranking.Count > 0)
        {
            if (rankingDropdown == null)
            {
                Debug.LogError("El Dropdown de Ranking no está asignado.");
                return;
            }

            int selectedCategoryId = rankingDropdown.value;

            // Si la opción seleccionada es "General" (index 0)
            List<attempt> filteredAttempts;

            if (selectedCategoryId == 0)  // General
            {
                // Mostrar todos los intentos sin filtrar por categoría
                filteredAttempts = SupabaseManager.Instance.ranking;
            }
            else
            {
                // Filtrar por category_id si se selecciona una categoría
                string selectedCategory = rankingDropdown.options[selectedCategoryId].text;
                filteredAttempts = SupabaseManager.Instance.ranking
     .FindAll(attempt => attempt.trivia_id == int.Parse(selectedCategory));

            }

            Debug.Log($"Filtrando intentos por categoría: {rankingDropdown.options[selectedCategoryId].text}");

            // Crear el ranking con los intentos filtrados
            CreateRanking(filteredAttempts);
        }
        else
        {
            Debug.Log("No hay intentos registrados en el ranking.");
        }
    }



}
