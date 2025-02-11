using UnityEngine;
using UnityEngine.UI;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    string supabaseUrl = "https://dxnralwsjyajjuvtklyh.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImR4bnJhbHdzanlhamp1dnRrbHloIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI2NTEyMjEsImV4cCI6MjA0ODIyNzIyMX0.ycX6tcetKLqeTYggvN4VDE6WGo2tZDSQ_1xoD12lTwA";

    Supabase.Client clientSupabase;
    public int index;
    public Image questionImage;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        index = PlayerPrefs.GetInt("SelectedIndex");

        await LoadTriviaData(index);
    }

    //Carga de preguntas segun trivia seleccionada
    async Task LoadTriviaData(int index)
    {
        if (clientSupabase == null)
        {
            Debug.LogError("Supabase client is not initialized.");
            return;
        }

        Debug.Log($"Cargando preguntas de la trivia: {index}");

        var triviaQuestion = await clientSupabase
            .From<question>()
            .Where(question => question.trivia_id == index)
            .Select("id, question, answer1, answer2, answer3, correct_answer, trivia_id, trivia(id, category),asset")
            .Get();

        if (triviaQuestion == null || triviaQuestion.Models.Count == 0)
        {
            Debug.LogError("No se encontraron preguntas del trivia dado");
            return;
        }
        else
        {
            Debug.Log($"Se encontraron {triviaQuestion.Models.Count} preguntas.");
        }

        GameManager.Instance.currentTriviaIndex = index;
        GameManager.Instance.questionList = triviaQuestion.Models;

        Debug.Log("Cantidad de pregutas cargadas: " + triviaQuestion.Models.Count);
        Debug.Log("preguntas guardadas en GameManager: " + GameManager.Instance.questionList.Count);


        if (triviaQuestion.Models.Count > 0)
        {
            StartCoroutine(LoadImage(triviaQuestion.Models[0].asset, () =>
            {
                UIManagment.Instance.LoadNextQuestion();
            }));
        }
        else
        {
            Debug.LogError("No hay preguntas disponibles.");
        }
    }

    //Carga de imagenes

    public IEnumerator LoadImage(string url, System.Action onImageLoaded)
    {
        if (string.IsNullOrEmpty(url))
        {
            if (questionImage != null)
            {
                questionImage.gameObject.SetActive(false);
            }
            Debug.Log("No hay asset.");
            onImageLoaded?.Invoke();
            yield break;
        }

        if (questionImage != null)
        {
            questionImage.gameObject.SetActive(true);
        }

        Debug.Log("URL de la imagen: " + url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);  //solicitud HTTP 
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (questionImage != null)
            {
                questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Debug.Log("Se cargo la imagen.");
            }
        }
        else
        {
            Debug.LogError("Error al cargar imagen: " + www.error);
        }

        onImageLoaded?.Invoke();

    }
}
