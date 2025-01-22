using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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

    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

    Supabase.Client clientSupabase;
    void Start()
    {
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


        if (login_password.Model.password.Equals(_loginUserPassInput.text))
        {
            print("LOGIN SUCCESSFUL");
            _stateText.text = "LOGIN SUCCESSFUL";
            _stateText.color = Color.green;

            SceneManager.LoadScene("TriviaSelectScene");
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
            SceneManager.LoadScene("TriviaSelectScene");
        }
    }
}