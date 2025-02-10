using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class SupabaseManager : MonoBehaviour

{

    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _loginUserIDInput;
    [SerializeField] TMP_InputField _loginUserPassInput;
    [SerializeField] TMP_InputField _signUpUserIDInput;
    [SerializeField] TMP_InputField _signUpUserPassInput;
    [SerializeField] TextMeshProUGUI _signUpstateText;

    [SerializeField] TextMeshProUGUI _loginstateText;
    [SerializeField] TMP_Dropdown _ageDropdown;
    [SerializeField] GameObject signUpPopUp;

    public static SupabaseManager Instance { get; private set; }
    public static string CurrentUserName { get; private set; }

    public static int CurrentUserId { get; private set; }
    public static int NewAttemptId { get; private set; }

    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

    public List<attempt> ranking = new List<attempt>();
    public List<users> users = new List<users>();
    public List<trivia> trivias = new List<trivia>();

    Supabase.Client clientSupabase;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Instancia de SupabaseManager creada.");
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);


    }

    void Start()
    {

        InitializeData();
    }

    public Supabase.Client GetClientSupabase()
    {
        return clientSupabase;
    }

    async void InitializeData()
    {
        await LoadUsers();
        await LoadRanking();
        await LoadTrivias();
    }

    async Task LoadUsers()
    {
        var response = await clientSupabase.From<users>().Select("*").Get();
        if (response?.Models != null) users = response.Models;
    }

    async Task LoadRanking()
    {
        var response = await clientSupabase.From<attempt>().Select("*").Get();
        if (response?.Models != null) ranking = response.Models;
    }

    async Task LoadTrivias()
    {
        var response = await clientSupabase.From<trivia>().Select("*").Get();
        if (response?.Models != null) trivias = response.Models;
    }

    //Inicio de sesion 
    public async void UserLogin()
    {
        var test_response = await clientSupabase
            .From<users>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);

        var existingUser = await clientSupabase
    .From<users>()
    .Select("*")
    .Where(users => users.username == _loginUserIDInput.text)
    .Get();

        if (existingUser.Models.Count == 0)
        {
            print("Nombre incorrecto");
            _loginstateText.text = "Nombre incorrecto o inxesitente";
            _loginstateText.color = Color.red;
            return;
        }

        var login_password = await clientSupabase
          .From<users>()
          .Select("password")
          .Where(users => users.username == _loginUserIDInput.text)
          .Get();


        if (string.Equals(login_password.Model.password, _loginUserPassInput.text))
        {
            print("LOGIN SUCCESSFUL");
            _loginstateText.text = "LOGIN SUCCESSFUL";
            _loginstateText.color = Color.green;

            CurrentUserId = existingUser.Models[0].id;
            CurrentUserName = existingUser.Models[0].username;


            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            print("contraseña incorrecta");
            _loginstateText.text = "contraseña incorrecta";
            _loginstateText.color = Color.red;
        }
    }

    //Creacion de cuenta nueva

    public void ShowsignUpPopUp()
    {
        _signUpstateText.text = "";
        signUpPopUp.SetActive(true);
    }

    public void HidesignUpPopUp()
    {
        _signUpUserIDInput.text = "";
        _signUpUserPassInput.text = "";
        _signUpstateText.text = "";
        _ageDropdown.value = 0;
        signUpPopUp.SetActive(false);

    }

    public async void InsertNewUser()
    {

        Debug.Log("Método InsertNewUser llamado");

        int selectedAge = int.Parse(_ageDropdown.options[_ageDropdown.value].text);

        if (string.IsNullOrEmpty(_signUpUserIDInput.text) || string.IsNullOrEmpty(_signUpUserPassInput.text))
        {
            _signUpstateText.text = "Por favor, complete todos los campos";
            _signUpstateText.color = Color.red;
            return;
        }

        var existingUser = await clientSupabase
        .From<users>()
        .Select("*")
          .Where(users => users.username == _signUpUserIDInput.text)
        .Get();

        if (existingUser.Models.Count > 0)
        {
            _signUpstateText.text = "El nombre de usuario ya está en uso";
            _signUpstateText.color = Color.red;
            return;
        }

        var ultimoId = await clientSupabase
         .From<users>()
         .Select("id")
         .Order("id", Postgrest.Constants.Ordering.Descending)
         .Get();

        int nuevoId = 1;

        if (ultimoId.Models.Count > 0 && ultimoId.Models[0] != null)
        {
            nuevoId = ultimoId.Models[0].id + 1;
        }

        var nuevoUsuario = new users
        {
            id = nuevoId,
            username = _signUpUserIDInput.text,
            age = selectedAge,
            password = _signUpUserPassInput.text,
        };

        var resultado = await clientSupabase
            .From<users>()
            .Insert(new[] { nuevoUsuario });

        if (resultado != null && resultado.Models.Count > 0)
        {
            _signUpstateText.text = "Usuario Correctamente Ingresado. Cierre la ventana e inicie sesion";
            _signUpstateText.color = Color.green;
        }
    }

    //Guardado del intento y organizacion del ranking

    public async void SaveAttempt(int id, int userId, int triviaId, int score, int correct_answercount, float time)
    {
        int totalTime = Mathf.RoundToInt(Timer.Instance.GetGameTime());
        Debug.Log($"Intentando guardar intento: userId={userId}, triviaId={triviaId}, score={score}, correct_answercount={correct_answercount}, time={time}");
        if (userId == 0 || triviaId == 0)
        {
            Debug.LogError("User ID o Trivia ID no disponibles.");
            return;
        }
        var lastAttemptId = await clientSupabase
        .From<attempt>()
        .Select("id")
        .Order(attempt => attempt.id, Postgrest.Constants.Ordering.Descending)
        .Get();
        int NewAttemptId = 1;

        if (lastAttemptId.Models.Count > 0 && lastAttemptId.Models[0] != null)
        {
            NewAttemptId = lastAttemptId.Models[0].id + 1;
        }

        var newAttempt = new attempt
        {
            id = NewAttemptId,
            users_id = userId,
            trivia_id = triviaId,
            score = score.ToString(),
            correct_answercount = correct_answercount,
            time = totalTime.ToString(),
        };

        var insertResponse = await clientSupabase
            .From<attempt>()
            .Insert(new[] { newAttempt });

        if (insertResponse.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Intento guardado exitosamente.");
        }
        else
        {
            Debug.Log("Error al guardar el intento." + insertResponse.ResponseMessage);
        }
    }

    public async Task<List<attempt>> OrderScore(int? triviaId = null)
    {
        await LoadRanking();

        var sortedList = ranking.OrderByDescending(x => int.Parse(x.score)).ToList();

        if (triviaId.HasValue)
        {
            sortedList = sortedList.Where(x => x.trivia_id == triviaId.Value).ToList();
        }
        Debug.Log($"Intentos filtrados por triviaId={triviaId}: {sortedList.Count}");
        return sortedList.Take(10).ToList();
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

}