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

        GameManager.Instance.currentTriviaIndex = index;
        GameManager.Instance.responseList = response.Models;
        Debug.Log("Response from query: " + response.Models.Count);
        Debug.Log("ResponseList from GM: " + GameManager.Instance.responseList.Count);

        foreach (var question in response.Models)
        {
            string assetUrl = question.asset;
            Debug.Log("Asset URL for question ID " + question.Id + ": " + assetUrl);
            StartCoroutine(LoadImage(assetUrl));
        }

        UIManagment.Instance.LoadNextQuestion();
    }

    public IEnumerator LoadImage(string url)
    {

        if (string.IsNullOrEmpty(url))
        {
            // Si no hay URL, desactivar la imagen y salir
            if (questionImage != null)
            {
                questionImage.gameObject.SetActive(false);
            }
            Debug.LogWarning("No image URL provided.");
            yield break; // Salir de la coroutine si no hay imagen
        }

        // Si hay URL, activar la imagen
        if (questionImage != null)
        {
            questionImage.gameObject.SetActive(true);
        }
        Debug.Log("Encoded URL: " + url);  // Verifica la URL codificada

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
            if (UnityWebRequest.Result.ConnectionError == www.result || UnityWebRequest.Result.ProtocolError == www.result)
            {
                Debug.LogError("Error loading image: " + www.error);
                yield break;
            }
        }
    }
}
