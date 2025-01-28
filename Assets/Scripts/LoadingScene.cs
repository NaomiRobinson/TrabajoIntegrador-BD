using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{

    [SerializeField] private Slider loadingBar; // Si tienes una barra de carga
    [SerializeField] private Text loadingText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    private IEnumerator LoadMainScene()
    {
        // Comienza la carga asíncrona de la escena principal
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main");
        operation.allowSceneActivation = false;

        StartCoroutine(LoadQuestionData());

        while (!operation.isDone)
        {
            // Actualiza la barra de carga
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar != null)
                loadingBar.value = progress;
            if (loadingText != null)
                loadingText.text = "Cargando... " + (progress * 100f).ToString("F0") + "%";


            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator LoadQuestionData()
    {
        // Simulando un retraso en la carga de datos
        yield return new WaitForSeconds(2f); // Cambia esto con la carga real de los datos.

        // Aquí debes obtener la pregunta desde la base de datos y asignarla a tu UI
        Debug.Log("Pregunta cargada desde la base de datos.");
    }

    void Update()
    {

    }
}
