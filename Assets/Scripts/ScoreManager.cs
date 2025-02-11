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

    [SerializeField] GameObject rankingItemPrefab;
    [SerializeField] Transform contentTransform;
    [SerializeField] TMP_Dropdown rankingDropdown;
    [SerializeField] private Button backButton;
    [SerializeField] GameObject loadingSpinner;

    private List<trivia> trivias;


    public static ScoreManager Instance { get; private set; }



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        FillRankingDropdown();
        rankingDropdown.onValueChanged.AddListener(DropdownCategoryChanged);

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackMenu);
        }

        await ShowRanking();
    }

    void Update()
    {
        if (loadingSpinner != null && loadingSpinner.activeInHierarchy)
        {
            loadingSpinner.transform.Rotate(0f, 0f, -100f * Time.deltaTime);
        }
    }

    public void FillRankingDropdown()
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

    public async void DropdownCategoryChanged(int index)
    {
        string selectedCategory = rankingDropdown.options[index].text;
        Debug.Log("Categoría seleccionada: " + selectedCategory); 

        int? triviaId = selectedCategory == "General" ? null : SupabaseManager.Instance.GetTriviaIdFromCategory(selectedCategory);
        await ShowRanking(triviaId);

    }
    
 public async Task ShowRanking(int? triviaId = null)
    {
        Debug.Log("ShowRanking ha sido llamada.");
        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(true);
        }

        List<attempt> filteredAttempts = await SupabaseManager.Instance.OrderScore(triviaId);
        

        foreach (var attempt in filteredAttempts)
        {
            Debug.Log($"Intento - Usuario: {attempt.users_id}, Puntuación: {attempt.score}, Trivia ID: {attempt.trivia_id}");
        }

        await ReloadUsers();
        CreateRanking(filteredAttempts);

        if (loadingSpinner != null)
        {
            loadingSpinner.SetActive(false);
        }
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
            texts[1].text = username; 
            texts[2].text = attempt.score.ToString(); 

            position++;
        }
    }


    public void BackMenu()
    {
        rankingDropdown.onValueChanged.RemoveListener(DropdownCategoryChanged);
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }


    public async Task ReloadUsers()
    {
        var userData = await SupabaseManager.Instance.GetClientSupabase()
            .From<users>()
            .Select("*")
            .Get();

        SupabaseManager.Instance.users = userData.Models;

        Debug.Log("Usuarios recargados:");
        foreach (var u in SupabaseManager.Instance.users)
        {
            Debug.Log($"ID: {u.id}, Nombre: {u.username}");
        }
    }


}
