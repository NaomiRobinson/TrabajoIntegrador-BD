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

public class SupabaseManager : MonoBehaviour

{

    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _loginUserIDInput;
    [SerializeField] TMP_InputField _loginUserPassInput;
    [SerializeField] TMP_InputField _signUpUserIDInput;
    [SerializeField] TMP_InputField _signUpUserPassInput;
    [SerializeField] TextMeshProUGUI _stateText;
    [SerializeField] TMP_Dropdown _ageDropdown;
    [SerializeField] GameObject signUpPopUp;

    public static SupabaseManager Instance { get; private set; }

    public static int CurrentUserId { get; private set; }
    public static int NewAttemptId { get; private set; }

    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

    public List<attempt> ranking = new List<attempt>();

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
            _stateText.text = "Nombre incorrecto";
            _stateText.color = Color.red;
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
            _stateText.text = "LOGIN SUCCESSFUL";
            _stateText.color = Color.green;

            CurrentUserId = existingUser.Models[0].id;

            //PlayerPrefs.SetInt("user_id", existingUser.Models[0].id);
            //PlayerPrefs.Save();

            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            print("contraseña incorrecta");
            _stateText.text = "contraseña incorrecta";
            _stateText.color = Color.red;
        }
    }

    public void ShowsignUpPopUp()
    {
        _stateText.text = "";
        signUpPopUp.SetActive(true);
    }

    public void HidesignUpPopUp()
    {
        _signUpUserIDInput.text = "";
        _signUpUserPassInput.text = "";
        _stateText.text = "";
        _ageDropdown.value = 0;
        signUpPopUp.SetActive(false);

    }


    public async void InsertNewUser()
    {

        Debug.Log("Método InsertNewUser llamado");

        int selectedAge = int.Parse(_ageDropdown.options[_ageDropdown.value].text);

        if (string.IsNullOrEmpty(_signUpUserIDInput.text) || string.IsNullOrEmpty(_signUpUserPassInput.text))
        {
            _stateText.text = "Por favor, complete todos los campos";
            _stateText.color = Color.red;
            return;
        }


        var existingUser = await clientSupabase
        .From<users>()
        .Select("*")
          .Where(users => users.username == _signUpUserIDInput.text)
        .Get();

        if (existingUser.Models.Count > 0)
        {
            _stateText.text = "El nombre de usuario ya está en uso";
            _stateText.color = Color.red;
            return;
        }

        var ultimoId = await clientSupabase
         .From<users>()
         .Select("id")
         .Order(users => users.id, Postgrest.Constants.Ordering.Descending)
         .Get();

        int nuevoId = 1;



        if (ultimoId.Models.Count > 0 && ultimoId.Models[0] != null)
        {
            nuevoId = ultimoId.Models[0].id + 1;
        }

        // Crear el nuevo usuario con el nuevo id
        var nuevoUsuario = new users
        {
            id = nuevoId,
            username = _signUpUserIDInput.text,
            age = selectedAge,
            password = _signUpUserPassInput.text,
        };

        // Insertar el nuevo usuario
        var resultado = await clientSupabase
            .From<users>()
            .Insert(new[] { nuevoUsuario });

        // Verificar el estado de la inserción
        if (resultado != null && resultado.Models.Count > 0)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = Color.green;
        }
    }

    public async void SaveAttempt(int id, int userId, int triviaId, int score, int correct_answercount, float time)
    {
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

        // Crear un nuevo intento
        var newAttempt = new attempt
        {
            id = NewAttemptId,
            users_id = userId,
            trivia_id = triviaId,
            score = score.ToString(),
            correct_answercount = correct_answercount,
            time = time.ToString()
        };

        // Insertar el nuevo intento en la base de datos
        var insertResponse = await clientSupabase
            .From<attempt>()
            .Insert(new[] { newAttempt });

        if (insertResponse != null && insertResponse.Models.Count > 0)
        {
            Debug.Log("Intento guardado exitosamente.");
        }
        else
        {
            Debug.Log("Error al guardar el intento.");
        }
    }

    public async Task OrderScore()
    {
        var ranking = await clientSupabase
        .From<attempt>()
        .Select("id,score,time,correct_answercount,trivia_id,users_id")
        .Order("score", Postgrest.Constants.Ordering.Descending)
        .Limit(10)
        .Get();

        foreach (var attempt in ranking.Models)
        {
            var user = await clientSupabase
                .From<users>()
                .Select("*")
                .Where(u => u.id == attempt.users_id)
                .Get();


            attempt.user = user.Models[0];
        }
        if (ranking.Models != null)
        {
            this.ranking = ranking.Models;
        }
    }

    public Supabase.Client GetClientSupabase()
    {
        return clientSupabase;
    }


}