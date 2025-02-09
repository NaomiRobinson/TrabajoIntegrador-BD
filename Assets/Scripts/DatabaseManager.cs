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

    // private string bucketName = "assets";

    Supabase.Client clientSupabase;


    public int index;
    public Image questionImage;
    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        index = PlayerPrefs.GetInt("SelectedIndex");

        await LoadTriviaData(index);
    }

    async Task LoadTriviaData(int index)
    {
        if (clientSupabase == null)
        {
            Debug.LogError("Supabase client is not initialized.");
            return;
        }

        Debug.Log($"Loading trivia data for index: {index}");

        var response = await clientSupabase
            .From<question>()
            .Where(question => question.trivia_id == index)
            .Select("id, question, answer1, answer2, answer3, correct_answer, trivia_id, trivia(id, category),asset")
            .Get();

        if (response == null || response.Models.Count == 0)
        {
            Debug.LogError("No trivia data found for the given index.");
            return;
        }
        else
        {
            Debug.Log($"Found {response.Models.Count} trivia items.");
        }

        GameManager.Instance.currentTriviaIndex = index;
        GameManager.Instance.responseList = response.Models;

        Debug.Log("Response from query: " + response.Models.Count);
        Debug.Log("ResponseList from GM: " + GameManager.Instance.responseList.Count);

        // Solo cargar la imagen de la primera pregunta
        if (response.Models.Count > 0)
        {
            StartCoroutine(LoadImage(response.Models[0].asset, () =>
            {
                UIManagment.Instance.LoadNextQuestion();
            }));
        }
        else
        {
            Debug.LogError("No questions available to load.");
        }
    }

    public IEnumerator LoadImage(string url, System.Action onImageLoaded)
    {
        if (string.IsNullOrEmpty(url))
        {
            if (questionImage != null)
            {
                questionImage.gameObject.SetActive(false);
            }
            Debug.LogWarning("No image URL provided.");
            onImageLoaded?.Invoke(); // Asegurar que el callback se llama incluso si no hay imagen
            yield break;
        }

        if (questionImage != null)
        {
            questionImage.gameObject.SetActive(true);
        }
        Debug.Log("Encoded URL: " + url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (questionImage != null)
            {
                questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                Debug.Log("Image loaded successfully.");
            }
        }
        else
        {
            Debug.LogError("Error loading image: " + www.error);
        }


        onImageLoaded?.Invoke();

    }
}
