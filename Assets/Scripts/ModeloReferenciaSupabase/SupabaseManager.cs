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
    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;

    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

    Supabase.Client clientSupabase;



    public async void UserLogin()
    {
        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // prueba
        var test_response = await clientSupabase
            .From<users>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);



        // filtro seg�n datos de login
        var login_password = await clientSupabase
          .From<users>()
          .Select("password")
          .Where(users => users.username == _userIDInput.text)
          .Get();



        if (login_password.Model.password.Equals(_userPassInput.text))
        {
            print("LOGIN SUCCESSFUL");
            _stateText.text = "LOGIN SUCCESSFUL";
            _stateText.color = Color.green;
            SceneManager.LoadScene("TriviaSelectScene");
        }
        else
        {
            print("WRONG PASSWORD");
            _stateText.text = "WRONG PASSWORD";
            _stateText.color = Color.red;
        }
    }

    public async void InsertarNuevoUsuario()
    {
        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // Consultar el último id utilizado (ID = index)
        var ultimoId = await clientSupabase
            .From<users>()
            .Select("id")
            .Order(users => users.id, Postgrest.Constants.Ordering.Descending)
            .Get();

        int nuevoId = 1;

        if (ultimoId.Model != null)
        {

            nuevoId = ultimoId.Model.id + 1;
        }


        // Crear el nuevo usuario con el nuevo id
        var nuevoUsuario = new users
        {
            id = nuevoId,
            username = _userIDInput.text,
            age = Random.Range(0, 100), // Luego creo el campo que falta en la UI
            password = _userPassInput.text,
        };

        // Insertar el nuevo usuario
        var resultado = await clientSupabase
            .From<users>()
            .Insert(new[] { nuevoUsuario });

        // Verificar el estado de la inserción
        if (resultado != null && resultado.ResponseMessage != null && resultado.ResponseMessage.IsSuccessStatusCode)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = Color.green;
            SceneManager.LoadScene("TriviaSelectScene");
        }
        else
        {
            _stateText.text = "Error en el registro de usuario";
            if (resultado != null && resultado.ResponseMessage != null)
            {
                _stateText.text += "\n" + resultado.ResponseMessage.ToString();
            }
            _stateText.color = Color.red;
        }
    }
}

